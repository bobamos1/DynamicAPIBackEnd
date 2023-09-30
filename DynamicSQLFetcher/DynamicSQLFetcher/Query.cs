//using System;
//using System.Collections.Generic;
using Dapper;
using System;
using System.Text.RegularExpressions;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace DynamicSQLFetcher
{
    public class Query
    {
        internal QueryTypes queryType { get; set; }
        internal string query { get; set; }
        internal List<SQLVariableInfo> varsInfoList { get; set; }
        public Dictionary<string, bool> variablesInQuery { get; set; }
        internal Dictionary<string, string> selectColumns { get; set; }
        internal Dictionary<string, object> paramsUsed { get; set; }
        internal bool completeCheck { get; set; }
        public Query(QueryTypes queryType)
        {
            this.queryType = queryType;
            varsInfoList = new List<SQLVariableInfo>();
            variablesInQuery = new Dictionary<string, bool>();
            selectColumns = new Dictionary<string, string>();
            paramsUsed = new Dictionary<string, object>();
            completeCheck = false;
        }
        public Query setCompleteCheck(bool completeCheck)
        {
            this.completeCheck = completeCheck;
            return this;
        }
        public Query clearParams()
        {
            paramsUsed.Clear();
            return this;
        }
        public DynamicParameters getParameters()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            if (paramsUsed == null || paramsUsed.Count == 0)
                return dynamicParameters;
            foreach (var parameter in paramsUsed)
                dynamicParameters.Add(parameter.Key, parameter.Value);
            return dynamicParameters;
        }
        public Query addParams(params KeyValuePair<string, object>[] paramsUsed)
        {
            foreach (var param in paramsUsed)
                this.paramsUsed[param.Key] = param.Value;
            return this;
        }
        public Query addParams(string paramName, object paramValue)
        {
            this.paramsUsed[paramName] = paramValue;
            return this;
        }
        public Query addParams(Dictionary<string, object> paramsUsed)
        {
            object currentVar;
            foreach (var variable in variablesInQuery)
            {
                if (paramsUsed.TryGetValue(variable.Key, out currentVar))
                    this.paramsUsed[variable.Key] = currentVar;
            }
            return this;
        }
        public Query setParams(Dictionary<string, object> paramsUsed)
        {
            return clearParams().addParams(paramsUsed);
        }
        public Query setParam(string paramName, object paramValue)
        {
            this.paramsUsed[paramName] = paramValue;
            return this;
        }
        public Query removeParams(string paramName)
        {
            this.paramsUsed.Remove(paramName);
            return this;
        }
        private static readonly List<string> delimiters = new List<string>
        {
           "SELECT", "DISTINCT", "TOP", "FROM", "WHERE", "(", ")", "VALUES", "ORDER BY", "UPDATE", "SET"
        };
        private static readonly List<string> separators = new List<string>
        {
           ",", "AND", "OR"
        };
        private static readonly List<KeyValuePair<string, string>> emptyRemover = new List<KeyValuePair<string, string>>
        {
            new("AND)", ")"),
            new("OR)", ")"),
            new("AND )", ")"),
            new("OR)", ")"),
            new("()AND", ""),
            new("()OR", ""),
            new("() AND", ""),
            new("() OR", ""),
            new("()", ""),
            new("( )", ""),
            new("WHEREORDER", "ORDER"),
            new("WHEREGROUP", "GROUP"),
            new("WHERE)", ")"),
            new("WHERE )", ")")
        };
        private static readonly List<string> removeEnd = new List<string>()
        {
            "WHERE", "AND", "OR"
        };
        private static readonly Dictionary<char, char> replacements = new Dictionary<char, char>
        {
            { '@', '\u2030' },
            { '[', '\u2032' },
            { ']', '\u2033' },
            { ',', '\u2034' },
            { '~', '\u2035' },
            { '(', '\u2036' },
            { ')', '\u2037' },
            { '#', '\u2038' },
            { 'S', '\u2039' },
            { 'F', '\u2040' },
            { 'A', '\u2041' },
            { 'O', '\u2042' }
        };
        internal static readonly Dictionary<char, Func<int, string, string, Tuple<int, int, string, bool>>> variableType = new Dictionary<char, Func<int, string, string, Tuple<int, int, string, bool>>>
        {
            { '_', (i, variableName, completeQuery) => getStartEnd(i, variableName, completeQuery, 0, false) },
            { '#', (i, variableName, completeQuery) => getStartEnd(i, variableName, completeQuery, 1, false) },
            { '&', (i, variableName, completeQuery) => getStartEnd(i, variableName, completeQuery, 0, true) }
        };
        private static readonly string hilightStart = "\u2018\u2019\u2018";
        private static readonly string hilightEnd = "\u2019\u2018\u2019";
        private static readonly char delimiterStart = '\u2020';
        private static readonly char delimiterEnd = '\u2021';
        private static readonly char splitter = '\u2221';
        public string Parse(int page, int step, params string[] authorizedColumns)
        {
            if (queryType != QueryTypes.SELECT)
                throw new Exception("need to be of type select to add pagination");
            return string.Format("{0} {1}", Parse(authorizedColumns), getPagination(page, step));
        }
        public KeyValuePair<Query, string[]> AddCols(params string[] authorizedColumns)
        {
            return new KeyValuePair<Query, string[]>(this, authorizedColumns);
        }
        public string Parse(params string[] authorizedColumns)
        {
            string queryStr;
            if (authorizedColumns is null || !authorizedColumns.Any())
            {
                if (!selectColumns.Any())
                    queryStr = this.query;
                else if (queryType == QueryTypes.ARRAY || queryType == QueryTypes.VALUE)
                    queryStr = this.query.Replace(hilightStart + hilightEnd, string.Format(" {0} AS [item] ", this.selectColumns.First().Value));
                else if (queryType == QueryTypes.CBO)
                    queryStr = this.query.Replace(hilightStart + hilightEnd, string.Format(" {0} AS [key], {1} AS [value] ", this.selectColumns.First().Value, this.selectColumns.Last().Value));
                else
                    throw new Exception("selectColonnes can not be null");
            }
            else
                queryStr = SelectCols(authorizedColumns);
            queryStr = string.Format(queryStr, varsInfoList.Select(var => var.validateVar(paramsUsed)).ToArray());
            if (completeCheck)
                queryStr = validateAll(queryStr);
            return queryStr;
        }
        public string Parse(int page, int step)
        {
            if (queryType != QueryTypes.SELECT)
                throw new Exception("need to be of type select to add pagination");
            return string.Format("{0} {1}", Parse(), getPagination(page, step));
        }
        /*
        internal string ParseTotalAuth()
        {
            return Parse(this.selectColumns.Select(col => col.Key).ToArray());
        }*/
        private static string getPagination(int page, int step)
        {
            return string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (page - 1) * step, step);
        }
        private string SelectCols(params string[] authorizedColumns)
        {
            IEnumerable<string> selectedCols = selectColumns
                    .Where(item => authorizedColumns.Contains(item.Key) && checkIfOkParam(item.Value, paramsUsed))
                    .Select(item => item.Value);
            if (queryType == QueryTypes.CBO)
                selectedCols = selectedCols.Take(2).Select((col, index) => $"{col} AS {(index == 0 ? "[key]" : "[value]")}");
            string queryStr = this.query.Replace(hilightStart + hilightEnd, string.Format(" {0} ", string.Join(", ", selectedCols)));
            return queryStr;
        }
        private string validateAll(string completeQuery)
        {
            removeEnd.ForEach(ending =>
            {
                if (completeQuery.EndsWith(ending))
                    completeQuery = completeQuery.Substring(0, completeQuery.Length - (ending.Length + 1));
            });
            for (int i = 0; i < emptyRemover.Count; i++)
                completeQuery = completeQuery.Replace(emptyRemover[i].Key, emptyRemover[i].Value);

            return completeQuery;
        }
        private bool checkIfOkParam(string lookString, Dictionary<string, object> paramsUsed)
        {
            int index = ExtractFirstNumber(lookString);
            if (index == -1)
                return true;
            return paramsUsed.ContainsKey(varsInfoList[index].VarName);
        }
        public static Query fromQueryString(QueryTypes queryType, string query, bool completeCheck = true, bool completeAuth = false, bool removeVarIdentifier = true)
        {
            char[] tempNoComma = query.ToCharArray();
            handleReplacements('\'', tempNoComma);
            handleReplacements('[', ']', tempNoComma);
            query = new string(tempNoComma);
            query = ReplaceDigitsWithDoubleCurlyBraces(query);
            query = handleDelimiters(query);
            query = handleSeparators(query);
            if (!completeAuth)
                switch (queryType)
                {
                    case QueryTypes.SELECT:
                    case QueryTypes.VALUE:
                    case QueryTypes.ROW:
                    case QueryTypes.CBO:
                    case QueryTypes.ARRAY:
                        query = highlightSelectPart("SELECT", "FROM", query);
                        break;
                    case QueryTypes.UPDATE:
                        query = highlightUpdatePart("SET", query);
                        break;
                    default:
                        break;
                }
            Query newQuery = new Query(queryType);
            query = parseVariables(query, newQuery, removeVarIdentifier);

            int startSelectPart = query.IndexOf(hilightStart);
            int endSelectPart = query.IndexOf(hilightEnd);
            if (startSelectPart != -1 && endSelectPart != -1)
            {
                startSelectPart += hilightStart.Length;
                string selectPart = query.Substring(startSelectPart, endSelectPart - startSelectPart);
                query = string.Format("{0}{1}", query.Substring(0, startSelectPart), query.Substring(endSelectPart));
                int level = 0;
                List<List<char>> selectSection = new List<List<char>>();
                selectSection.Add(new List<char>());
                for (int i = 0; i < selectPart.Length; i++)
                {
                    if (selectPart[i] == '(')
                        level++;
                    if (selectPart[i] == ')')
                        level--;
                    if (level == 0 && selectPart[i] == ',')
                    {
                        selectSection.Add(new List<char>());
                        continue;
                    }
                    selectSection.Last().Add(selectPart[i]);
                    if (selectPart[i] == '}' && i + 3 < selectPart.Length && selectPart[i + 3] == '{')
                        selectSection.Add(new List<char>());
                }
                string currentSelectVar;
                string currentVarName = "";
                foreach (var str in selectSection)
                {
                    currentSelectVar = cleanString(new string(str.ToArray()).Trim()).Trim();
                    int index = currentSelectVar.LastIndexOf(" AS ");
                    if (queryType == QueryTypes.UPDATE)
                        currentVarName = getColNameUpdate(currentSelectVar, newQuery);
                    else
                        currentVarName = getAliasOrColName(currentSelectVar, queryType, index);
                    if (newQuery.selectColumns.ContainsKey(currentVarName))
                        throw new Exception("all selectColumns needs to be unique");
                    if ((QueryTypes.CBO == queryType || QueryTypes.VALUE == queryType || QueryTypes.ARRAY == queryType) && index > -1)
                        currentSelectVar = currentSelectVar.Substring(0, index);
                    newQuery.selectColumns.Add(currentVarName, currentSelectVar);
                }
            }
            query = cleanString(query);
            //newQuery.hasTop1 = conditionalTop(query);
            if ((queryType == QueryTypes.ROW || queryType == QueryTypes.VALUE) && !conditionalTop(query))
                query = query.Replace(hilightStart, string.Format("{0}{1}", " TOP (1)", hilightStart));
            if (newQuery.varsInfoList.Any() && completeCheck)
                newQuery.completeCheck = true;
            newQuery.query = query;
            return newQuery;
        }
        private static bool conditionalTop(string completeQuery)
        {
            return completeQuery.Contains(string.Format("{0}{1}", "(1)", hilightStart));
        }
        private static string getAliasOrColName(string selectSection, QueryTypes queryType, int index)
        {
            if (index != -1)
            {
                /*if (QueryType.CBO == queryType || QueryType.VALUE == queryType || QueryType.ARRAY == queryType)
                    return getColName(selectSection.Substring(0, index));*/
                string colName = selectSection.Substring(index + 4).Trim();
                if (colName.Last() == ']')
                    return colName.Substring(1, colName.Length - 2);
                return colName;
            }
            return getColName(selectSection);
        }
        private static int ExtractFirstNumber(string input)
        {
            // Define a regular expression to match {x} where x is a number
            string regexPattern = @"\{(\d+)\}";
            Match match = Regex.Match(input, regexPattern);

            if (match.Success && match.Groups.Count == 2)
            {
                int number;
                if (int.TryParse(match.Groups[1].Value, out number))
                {
                    return number;
                }
            }

            return -1; // Return -1 if no match or unable to parse the number
        }
        private static string getColNameUpdate(string selectSection, Query query)
        {
            int index = ExtractFirstNumber(selectSection);
            if (index != -1)
                selectSection = query.varsInfoList[index].ConditionalString = query.varsInfoList[index].ConditionalString.Trim();
            if (selectSection.EndsWith(','))
                selectSection = query.varsInfoList[index].ConditionalString = selectSection.Remove(selectSection.Length - 1);
            selectSection = selectSection.Substring(0, selectSection.IndexOf("="));
            return getColName(selectSection);
        }
        private static string getColName(string selectSection)
        {
            List<char> selectName = new List<char>();
            bool inQuote = false;
            for (int i = selectSection.Length - 1; i >= 0; i--)
            {
                if (selectSection[i] == ']')
                {
                    inQuote = true;
                    continue;
                }
                if (selectSection[i] == '[' || (!inQuote && selectSection[i] == '.'))
                    break;
                selectName.Insert(0, selectSection[i]);
            }
            return new string(selectName.ToArray()).Trim();
        }
        private static string cleanString(string completeQuery)
        {
            foreach (var pair in replacements)
                completeQuery = completeQuery.Replace(pair.Value, pair.Key);
            completeQuery = new string(completeQuery.Where(c => c != delimiterStart && c != delimiterEnd).ToArray());
            return completeQuery;
        }
        private static string parseVariables(string completeQuery, Query queryObj, bool removeVarIdentifier)
        {
            List<Tuple<int, int, string, bool>> indexes = new List<Tuple<int, int, string, bool>>();
            char optionalType;
            string variableName;
            for (int i = 0; i < completeQuery.Length; i++)
            {
                if (completeQuery[i] == '@')
                {
                    optionalType = ' ';
                    if (variableType.ContainsKey(completeQuery[i + 1]))
                        optionalType = completeQuery[i + 1];
                    variableName = new string(getUntilNext(i + (optionalType == ' ' ? 1 : 2), completeQuery, delimiterStart, ' ', ',').Where(c => c != delimiterStart && c != delimiterEnd).ToArray());
                    if (optionalType != ' ')
                        indexes.Add(variableType[optionalType](i, variableName, completeQuery));
                    if (!queryObj.variablesInQuery.ContainsKey(variableName))
                        queryObj.variablesInQuery.Add(variableName, optionalType > 0);
                    else if (optionalType == ' ')
                        queryObj.variablesInQuery[variableName] = false;
                }
            }
            string extractedValue;
            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                queryObj.varsInfoList.Add(new SQLVariableInfo(cleanString(completeQuery.Substring(indexes[i].Item1, indexes[i].Item2)), indexes[i].Item3, indexes[i].Item4));
                completeQuery = string.Format("{0}{1}{2}{3}{4}", completeQuery.Substring(0, indexes[i].Item1), '{', queryObj.varsInfoList.Count - 1, '}', completeQuery.Substring(indexes[i].Item1 + indexes[i].Item2));
            }
            if (removeVarIdentifier)
                queryObj.varsInfoList.ForEach(var => var.removeVarIdentifier());
            return completeQuery;
        }
        private static string getUntilNext(int startIndex, string completeQuery, params char[] nextChars)
        {
            int index = completeQuery.Length;
            for (int i = startIndex; i < completeQuery.Length; i++)
                if (nextChars.Contains(completeQuery[i]))
                {
                    index = i;
                    break;
                }
            return completeQuery.Substring(startIndex, index - startIndex);
        }
        private static string ReplaceDigitsWithDoubleCurlyBraces(string input)
        {
            // Use a regular expression to find and replace digits within curly braces
            return Regex.Replace(input, @"{(\d+)}", @"{{$1}}");
        }
        private static string highlightSelectPart(string searchStart, string searchEnd, string completeQuery)
        {
            KeyValuePair<int, int>? selectPartIndexes = findInbetweenSearchKeyword(searchStart, searchEnd, completeQuery);
            if (selectPartIndexes is not null)
            {
                int start = (int)selectPartIndexes?.Key;
                int end = (int)selectPartIndexes?.Value;
                start = followedBy(start, completeQuery);
                return completeQuery.Insert(end, hilightEnd).Insert(start, hilightStart);
            }
            return completeQuery;
        }
        private static int followedBy(int startIndex, string completeQuery)
        {
            bool otherWord = false;
            char endingLook = delimiterEnd;
            for (int i = startIndex + 1; i < completeQuery.Length; i++)
            {
                if (completeQuery[i] == ' ')
                    continue;
                if (completeQuery[i] == delimiterStart)
                {
                    otherWord = true;
                    continue;
                }
                if (completeQuery[i] != delimiterEnd && !otherWord)
                    return startIndex;
                if (completeQuery[i] == endingLook && otherWord)
                    return endingLook == ')' ? i + 1 : i;
                if (stringIs(completeQuery, i, "TOP"))
                    endingLook = ')';
            }
            return startIndex;
        }
        private static bool stringIs(string completeQuery, int index, string search)
        {
            for (int i = index, j = 0; i < completeQuery.Length; i++, j++)
            {
                if (j == search.Length)
                    return true;
                if (completeQuery[i] == search[j])
                    continue;
                return false;
            }
            return false;
        }
        private static string highlightUpdatePart(string search, string completeQuery)
        {
            int startUpdate = findFirstSearchKeyword(search, completeQuery);
            if (startUpdate == -1)
                return completeQuery;
            completeQuery = completeQuery.Insert(startUpdate + search.Length, hilightStart);
            startUpdate += search.Length + hilightStart.Length + 1;
            int level = 0;
            for (int i = startUpdate; i < completeQuery.Length - 1; i++)
            {
                if (completeQuery[i] == ')')
                    level++;
                if (completeQuery[i] == '(')
                    level--;
                if (level == 0 && completeQuery[i] == delimiterStart && completeQuery[i + 1] != delimiterEnd)
                {
                    completeQuery = completeQuery.Insert(i + 1, hilightEnd);
                    break;
                }
            }
            return completeQuery;
        }
        private static KeyValuePair<int, int>? findInbetweenSearchKeyword(string searchStart, string searchEnd, string completeQuery)
        {
            KeyValuePair<int, int> tempPair = new KeyValuePair<int, int>(
                findFirstSearchKeyword(searchStart, completeQuery) + searchStart.Length,
                findFirstSearchKeyword(searchEnd, completeQuery));
            if (tempPair.Key - searchStart.Length == -1 || tempPair.Value == -1)
                return null;
            return tempPair;
        }
        private static int findFirstSearchKeyword(string search, string completeQuery, int start = 0, int level = 0)
        {
            if (start > 0)
                start += search.Length;
            int ind = completeQuery.IndexOf(search, start);
            if (ind == -1)
                return ind;
            for (int i = ind; i >= start; i--)
            {
                if (completeQuery[i] == ')')
                    level++;
                if (completeQuery[i] == '(')
                    level--;
            }
            if (level == 0)
                return ind;
            return findFirstSearchKeyword(search, completeQuery, ind, level);
        }
        private static Tuple<int, int, string, bool> getStartEnd(int index, string varName, string completeQuery, short needToBeLeveled, bool isSQLText)
        {
            int level = 0;
            char delimiterActu = delimiterEnd;
            short posOrNeg = -1;
            index--;
            Func<int, bool> action = (i) =>
            {
                if (completeQuery[i + posOrNeg] == ')')
                {
                    level++;
                    return false;
                }
                if (completeQuery[i + posOrNeg] == '(')
                {
                    level--;
                    return false;
                }
                if (completeQuery[i] == delimiterActu && level == (needToBeLeveled * posOrNeg))
                    return true;
                return false;
            };
            int start = isFolowedByParentesis(needToBeLeveled, index, completeQuery, false) ? index : descending(1, index, action) + 1;
            delimiterActu = delimiterStart;
            level = 0;
            posOrNeg = 1;
            index += varName.Length + 3;
            int end = isFolowedByParentesis(needToBeLeveled, index, completeQuery, true) ? index : ascending(index, completeQuery.Length - 2, action);
            return new Tuple<int, int, string, bool>(start, end - start, varName, isSQLText);
        }
        private static bool isFolowedByParentesis(short needToBeLeveled, int index, string completeQuery, bool isAsc)
        {
            if (needToBeLeveled != 0)
                return false;
            return isAsc ?
                completeQuery[index + 1] == ')' :
                completeQuery[index - 1] == '(';

        }
        public static int ascending(int min, int max, Func<int, bool> action)
        {
            for (int i = min; i <= max; i++)
                if (action(i))
                    return i;
            return -1;
        }
        public static int descending(int min, int max, Func<int, bool> action)
        {
            for (int i = max; i >= min; i--)
                if (action(i))
                    return i;
            return -1;
        }
        private static string handleDelimiters(string completeQuery)
        {
            foreach (var delimiter in delimiters)
                completeQuery = completeQuery.Replace(delimiter, string.Format("{0}{1}{2}", delimiterStart, delimiter, delimiterEnd));
            return string.Format("{0}{1}{2}{3}{4}", delimiterStart, delimiterEnd, completeQuery, delimiterStart, delimiterEnd);
        }
        private static string handleSeparators(string completeQuery)
        {
            foreach (var delimiter in separators)
                completeQuery = completeQuery.Replace(delimiter, string.Format("{0}{1}{2}", delimiter, delimiterStart, delimiterEnd));
            return completeQuery;
        }
        private static void handleReplacements(char start, char[] str)
        {
            handleReplacements(start, null, str);
        }
        private static void handleReplacements(char start, char? end, char[] str)
        {
            bool inComma = false;
            Action<char> action = end is null ?
                (chr) => {
                    if (chr == start)
                        inComma = !inComma;
                }
            :
                (chr) => {
                    if (chr == start)
                        inComma = true;
                    if (chr == end)
                        inComma = false;
                };
            for (int i = 0; i < str.Length; i++)
            {
                action(str[i]);
                if (inComma && str[i] != start && replacements.ContainsKey(str[i]))
                    str[i] = replacements[str[i]];
            }
        }
    }
}
