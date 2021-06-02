using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpoV2
{
    public struct Token
    {
        public String name;
        public String type;
        public String val;        
    }


    class LexicalAnalyzer
    {
        public LinkedList<Token> tokenList = new LinkedList<Token>();

        public LinkedList<Token> GetTokenList()
        {
            return tokenList;
        }

        public void PushToken(String name, String type, String val)
        {
            Token token;
            token.name = name;
            token.type = type;            
            token.val = val;

            tokenList.AddLast(token);
        }

        public Token PopToken()
        {
            var res = tokenList.Last.Value;
            tokenList.RemoveLast();
            return res;
        }

        public void PrintTokenList()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
               // Console.WriteLine($"-{tokenList.}- Key word");
            }
        }

        public String DelSpc(String text)
        {
            return text.Replace(" ", "");
        }

        public void PrintException(String s)
        {
            Console.WriteLine("Exception: " + s);
        }

        public String GetCode()
        {
            String code = "";
            try
            {
                StreamReader sr = new StreamReader("C:\\Users\\Данил\\source\\repos\\SpoV2\\SpoV2\\CodeCsharp.txt");
                code = sr.ReadToEnd();

                sr.Close();
            }
            catch (Exception e)
            {
                PrintException(e.Message);
            }

            return code;

        }

        public bool CheckBrackets(string text)
        {
            char ch = '0';
            int parity = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ch = text[i];
                if (ch == '(' || ch == '{' || ch == '[')
                    parity++;
                if (ch == ')' || ch == '}' || ch == ']')
                    parity--;
            }
            if (parity != 0)
                return false;

            return true;
        }

        public void PrintList(LinkedList<Token> list)
        {
            LinkedList<Token> tkns = list;
            
            Token tkn;
            int n = tkns.Count;
            Console.WriteLine("|Name:               |Type:          |Value:                        |\n");
            Console.WriteLine("|--------------------|---------------|------------------------------|\n");
            for (int i = 0; i < n; i++)
            {                
                tkn = tkns.First.Value;
                tkns.RemoveFirst();
                //Console.WriteLine($"{tkn.name}{1, 10}| {tkn.type}\t\t|{tkn.val}\t\t|\n");
                Console.WriteLine("|{0, 20}|{1,15}|{2,30}|\n", tkn.name, tkn.type, tkn.val);
                Console.WriteLine("|--------------------|---------------|------------------------------|\n");
            }
        }

        public bool IsKeyWord(string text)
        {
            string[] keyWords = { "List", "int", "new", "foreach", "in", "if", "else", "for" };

            return keyWords.Contains(text);
        }

        public bool IsOperator(string text)
        {
            string[] operators = { "+=", "-=", "*=", "/=", "+", "-", "*", "/", "++", "--" };
            return operators.Contains(text);
        }

        public bool IsNumber(string text)
        {
            int x;
            return Int32.TryParse(text, out x);
        }

        public bool IsVariable(string text)
        {
            bool result = true;

            if (text.Length == 0 || !Char.IsLetter(text[0]))
                result = false;
            else
                foreach (char s in text)
                {
                    if (s != '_' && !Char.IsDigit(s) && !Char.IsLetter(s))
                        result = false;
                }
            return result;
        }

        public bool IsSeporator(string text)
        {
            string[] seporators = { ";", "(", ")", " ", "\n", "\t", ",", "{", "}", "\r", "[", "]" };
            return seporators.Contains(text);
        }

        public bool IsAssigment(string text)
        {
            return (text == "=");
        }

        public bool IsComment(string text)
        {
            return (text == "//");
        }

        public bool IsCompare(string text)
        {
            string[] compare = { ">", "<", "<=", ">=", "==" };
            return compare.Contains(text);

        }

        public void CheckSeporators(char ch)
        {
            switch (ch)
            {
                case '(':
                    PushToken(ch + "", "CircleBracket", "null");
                    break;
                case ')':
                    PushToken(ch + "", "CircleBracket", "null");
                    break;
                case '{':
                    PushToken(ch + "", "FigureBracket", "null");
                    break;
                case '}':
                    PushToken(ch + "", "FigureBracket", "null");
                    break;
                case ',':
                    PushToken(ch + "", "Comma", "null");
                    break;
                case ';':
                    PushToken(ch + "", "Semicolon", "null");
                    break;
                default:

                    break;

            }

        }

        public void GetTokens(String code)
        {
            String subString = "";       
            
            char ch = '\0';

            for (int i = 0; i < code.Length; i++)
            {
                ch = code[i];

                if (code[i] == '/' && code[i + 1] == '/')
                {
                    String comment = "";
                    while (code[i] != '\n')
                    {
                        comment += code[i];
                        i++;
                    }
                    PushToken(comment, "Comment", "null");
                    subString = "";
                }

                if (!IsSeporator(ch + ""))
                    subString += ch;
                else
                {
                    if (IsSeporator("" + ch))
                    {
                        

                        if (IsKeyWord(DelSpc(subString)))
                        {
                            if (DelSpc(subString) == "int")
                            {
                                String variableType = DelSpc(subString);

                                PushToken(variableType, "KeyWord", "null");

                                subString = "";
                                do
                                {
                                    i++;
                                } while (!Char.IsLetter(code[i]));
                                    
                                while (code[i] != ' ')
                                {
                                    subString += code[i];
                                    i++;
                                }
                                String variableName = DelSpc(subString);
                                if (!IsVariable(variableName))
                                {
                                    PrintException("Variable name incorrect");
                                    goto EndGetToken;
                                }
                                subString = "";

                                String variableValue = "";
                                int stopCount = i;
                                while (true)
                                {
                                    //GetVarArg:
                                    if (code[i] == '=')
                                    {
                                        i++;
                                        while (code[i] != ';')
                                        {
                                            subString += code[i];
                                            i++;
                                        }
                                        variableValue = DelSpc(subString);
                                        PushToken(variableName, "Variable", variableValue);
                                        PushToken("=", "Assigment", "null");
                                        //PushToken(variableValue, "Number", "null");
                                        subString = "";
                                        i = stopCount + 1;
                                        break;
                                    }
                                    i++;
                                    if ((i - stopCount) > 4)
                                    {
                                        variableValue = "null";
                                        PushToken(variableName, "Variable", "null");
                                        i = stopCount;
                                        break;
                                    }
                                }
                            }
                        }
                        if (IsKeyWord(DelSpc(subString)))
                        {
                            PushToken(subString, "Keyword", "null");
                            subString = "";
                        }
                        else if (IsNumber(subString))
                        {
                            PushToken(subString, "Number", "null");
                            subString = "";
                        }
                        else if (IsOperator(subString))
                        {
                            PushToken(subString, "Ooerator", "null");
                            subString = "";
                        }
                        else if (IsCompare(subString))
                        {
                            PushToken(subString, "Compare", "null");
                            subString = "";
                        }
                        else
                        {
                            List<Token> list = tokenList.ToList<Token>();
                            Token t = list.Find(item => item.name == subString);
                            if (t.name != null)
                            {
                                //PushToken(t.name, t.type, t.val);
                                subString = "";
                                String variableValue = "";
                                String variableName = t.name;
                                int stopCount = i;
                                while (true)
                                {
                                    if (code[i] == '=')
                                    {
                                        int saveI = i;
                                        i++;
                                        while (code[i] != ';')
                                        {
                                            subString += code[i];
                                            i++;
                                        }
                                        variableValue = subString;
                                        PushToken(variableName, "Variable", variableValue);
                                        PushToken("=", "Assigment", "null");
                                        //PushToken(variableValue, "Number", "null");
                                        i = saveI + 1;
                                        subString = "";
                                        break;
                                    }
                                    i++;
                                    if ((i - stopCount) > 4)
                                    {
                                        variableValue = "null";
                                        PushToken(variableName, "Variable", "null");
                                        i = stopCount;
                                        break;
                                    }
                                }
                            }
                                

                        }
                        CheckSeporators(code[i]);
                    }
                }
            }
        EndGetToken:;
        }

    }

    class SyntaxAnalyzer
    {
        public LinkedList<Token> tokens;
        public LinkedList<Token> operationStack = new LinkedList<Token>();
        public LinkedList<Token> resultStack = new LinkedList<Token>();
        public String branchTab = "";
        public int tempI = 0;
        

        public SyntaxAnalyzer(LinkedList<Token> tkns)
        {
            this.tokens = tkns;
        }
                
        public void ToUpBranchTab()
        {
            branchTab += "\t";
        }

        public void ToDownBranchTab()
        {
            branchTab = branchTab.Substring(0, branchTab.Length - 1);
            
        }

        public bool IsContainsOperators(Token[] arr, int i)
        {
            int j = i;
            bool res = false;
            while (arr[j].name != ";")
            {
                if (arr[j].name == "+"
                    || arr[j].name == "-"
                    || arr[j].name == "*"
                    || arr[j].name == "/")
                {
                    res = true;
                }
                j++;
            }
            return res;
        }

        public bool IsMathOperator(Token tkn)
        {
            return tkn.name == "+" || tkn.name == "-" || tkn.name == "*" || tkn.name == "/";
        }

        public int GetOpPriority(Token tkn)
        {
            int res = 0;

            switch (tkn.name)
            {
                case "+":
                    res = 2;
                    break;
                case "-":
                    res = 2;
                    break;
                case "*":
                    res = 3;
                    break;
                case "/":
                    res = 3;
                    break;
                case "(":
                    res = 1;
                    break;

            }
            return res;

        }

        public void AnalyzeExpression (Token[] arr, int i)
        {
            
            String curret = "";
            while (true)
            {
                i++;
                curret = arr[i].name;

                if (curret == ";")
                {
                    tempI = i;
                    break;
                }
                    

                if (curret != "+" &&
                    curret != "-" &&
                    curret != "*" &&
                    curret != "/" &&
                    curret != "(" &&
                    curret != ")" )
                {
                    resultStack.AddLast(arr[i]);
                }
                else
                {
                    if (curret == "(")
                        operationStack.AddLast(arr[i]);
                    else
                    {
                        if (curret != ")")
                        {
                            if ((operationStack.Count == 0) || GetOpPriority(operationStack.Last.Value) < GetOpPriority(arr[i]))
                            {
                                operationStack.AddLast(arr[i]);
                            }
                            else
                            {
                                if (GetOpPriority(operationStack.Last.Value) >= GetOpPriority(arr[i]))
                                {
                                    while (operationStack.Count != 0)
                                    {

                                        if (GetOpPriority(operationStack.Last.Value) >= GetOpPriority(arr[i]))
                                        {
                                            resultStack.AddLast(operationStack.Last.Value);
                                            operationStack.RemoveLast();
                                        }
                                        if (operationStack.Count != 0 && GetOpPriority(operationStack.Last.Value) < GetOpPriority(arr[i]))
                                        {
                                            operationStack.AddLast(arr[i]); // ---------edited
                                            break;
                                        }
                                            
                                        if (operationStack.Count == 0 || GetOpPriority(operationStack.Last.Value) < GetOpPriority(arr[i]))
                                        {
                                            
                                            operationStack.AddLast(arr[i]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (curret == ")")
                    {
                        while (!(operationStack.Last.Value.name == "("))
                        {
                            if (operationStack.Last.Value.name == "(" || operationStack.Last.Value.name == ")")
                            {
                                
                                operationStack.RemoveLast();
                            }
                            else
                            {
                                
                                resultStack.AddLast(operationStack.Last.Value);
                                operationStack.RemoveLast();
                            }
                        }
                        operationStack.RemoveLast();
                        
                    }
                }
            }
            if (operationStack.Count != 0)
            {
                while (operationStack.Count != 0)
                {
                    
                    resultStack.AddLast(operationStack.Last.Value);
                    operationStack.RemoveLast();
                }
            }
        }

        public String RecursivePrintTree(Node[] arr, int i)
        {
            String res = "";
            Node cur = arr[i];
            if (i >= arr.Length)
                return res;
            if (cur == null)
            {
                return res;
            }
            else
            {
                ToUpBranchTab();
                res += branchTab + "\\__[" + cur.tkn.name + "]\n";
            }

            
            if (cur.right != null)
            {
                //ToUpBranchTab();
                res += RecursivePrintTree(arr, cur.right.id);
                ToDownBranchTab();
            }
            if (cur.left != null)
            {
                //ToUpBranchTab();
                res += RecursivePrintTree(arr, cur.left.id);
                ToDownBranchTab();                
            }

            if (cur.left == null && cur.right == null)
            {
                return res;
            }            

            return res;

        }

        public String AnalyzeAssign(Token[] arr, int i)
        {
            String res = "";
            int argsCount = 0;
            bool isContainsOperators = IsContainsOperators(arr, i);
            //int j = i;
            if (!isContainsOperators)
            {
                res =  branchTab + "---[" + arr[i - 1].name + "]\n" + branchTab + "\\__[" + arr[i + 1].name + "]\n";
                ToDownBranchTab();
                ToDownBranchTab();
            }
            else
            {
                AnalyzeExpression(arr, i);
                i = tempI;
                MyTree tree = new MyTree(resultStack);
                tree.FillTree();
                res = branchTab + RecursivePrintTree(tree.NodeArr, 0);

            }
            return res;
        }      

        public String AnalyzeConditions(Token[] arr, int i)
        {
            String res = "";
            int argsCount = 0;
            bool isContainsOperators = IsContainsOperators(arr, i);
            //int j = i;
            if (!isContainsOperators)
            {
                res = branchTab + "-----[" + arr[i - 1].name + "]\n" + branchTab + "\\____[" + arr[i + 1].name + "]\n";
                ToDownBranchTab();
                ToDownBranchTab();
            }
            else
            {
                //AnalyzeExpression(arr, i);
            }


            return res;

        }

        public String RecursiveTree(int i, String lastToken )
        {

            String output = "";

            Token[] tokensArray = tokens.ToArray();
            //int i = -1;

            while (true)
            {
                i++;

                String curTok = tokensArray[i].name;

                if (!(i + 1 < tokensArray.Length) && lastToken == "-")
                    break;
                else if (tokensArray[i].name == lastToken)
                {
                    break;
                }
                
                if (tokensArray[i].name == "int")
                {
                    output += branchTab + "[Declare] \n" + branchTab + "Type: " + tokensArray[i].name + "\n";
                    ToUpBranchTab();
                }
                else if (tokensArray[i].name == "=")
                {
                    output += branchTab + "[=]\n";
                    ToUpBranchTab();
                    output += AnalyzeAssign(tokensArray, i);
                }
                else if (tokensArray[i].name == "for")
                {
                    output += branchTab + "[for]\n";
                    ToUpBranchTab();
                    output += branchTab + "Conditions and Params:\n";
                    ToUpBranchTab();
                    output += RecursiveTree(i, ")");
                    i = tempI; 
                    //ToDownBranchTab();
                    output += branchTab + "Block:\n";
                    ToUpBranchTab();
                } else if (tokensArray[i].name == "<" 
                    || tokensArray[i].name == ">" 
                    || tokensArray[i].name == "==")
                {
                    output += branchTab + "[" + tokensArray[i].name + "]" + "\n";
                    ToUpBranchTab();
                    output += AnalyzeConditions(tokensArray, i);
                }
                
                
            }
            tempI = i;
            return output;
        }

        public void MakeTree(String code)
        {
            
            String output = "";
            
            Token[] tokensArray = tokens.ToArray();
            int i = -1;
            
            while (true)
            {
                i++;
                if (tokensArray[i].name == "int")
                {
                    output += branchTab + "[Declare] \n" + branchTab + "Type: " + tokensArray[i].name + "\n";
                    ToUpBranchTab();
                }else if(tokensArray[i].name == "=")
                {
                    output += branchTab + "[=]\n";
                    ToUpBranchTab();
                    output += AnalyzeAssign(tokensArray, i);
                }else if(tokensArray[i].name == "for")
                {
                    output += branchTab + "[for]\n";
                    ToUpBranchTab();
                    output += branchTab + "Conditions and Params:\n";
                    ToUpBranchTab();
                }

                if (!(i + 1 < tokensArray.Length))
                    break;
            }
            Console.Write(output);
        }
        
    }

    class Node
    {
        public Node parent, left, right;
        public Token tkn;
        public int id;


        public Node(Token t, int id)
        {
            this.tkn = t;
            this.id = id;
        }
    }

    class MyTree
    {
        public Node[] NodeArr;
        public Node CurNode;
        public LinkedList<Token> resultStack;

        public MyTree(LinkedList<Token> resultStack )
        {
            NodeArr = new Node[resultStack.Count];
            this.resultStack = resultStack;
        }

        public bool IsMathOperator(Token tkn)
        {
            return tkn.name == "+" || tkn.name == "-" || tkn.name == "*" || tkn.name == "/";
        }

        public int GetNextI()
        {
            int res = 0;

            while (NodeArr[res] != null )
            {
                res++;
            }

            return res;
        }

        public void FillTree()
        {
            CurNode = null;
            int n = resultStack.Count;
            for (int i = 0; i < n; i++)
            {
                
                if (CurNode == null)
                {
                    if (IsMathOperator(resultStack.Last.Value) && resultStack.Count != 0 && NodeArr[0] == null)
                    {
                        NodeArr[i] = new Node(resultStack.Last.Value, i);
                        resultStack.RemoveLast();
                    }
                    else if (IsMathOperator(resultStack.Last.Value) && resultStack.Count != 0 && NodeArr[i - 1] != null)
                    {
                        NodeArr[i - 1].right = new Node(resultStack.Last.Value, i); //присваиваем правой ноды родителя ссылку на новую текущую ноду
                        NodeArr[i] = NodeArr[i - 1].right; // текущей ветви присваиваем новую ветвь
                        NodeArr[i].parent = NodeArr[i - 1];  // присваиваем ссылку на родителя текущей ноды
                        resultStack.RemoveLast();
                    }
                    else if (!IsMathOperator(resultStack.Last.Value) && resultStack.Count != 0)
                    {
                        if (NodeArr[i - 1].right == null)
                        {
                            NodeArr[i - 1].right = new Node(resultStack.Last.Value, i); //
                            NodeArr[i] = NodeArr[i - 1].right;
                            NodeArr[i].parent = NodeArr[i - 1];
                            resultStack.RemoveLast();
                            i++;
                            NodeArr[i - 2].left = new Node(resultStack.Last.Value, i);
                            NodeArr[i] = NodeArr[i - 2].left;
                            NodeArr[i].parent = NodeArr[i - 2];
                            resultStack.RemoveLast();
                            if (NodeArr[i].parent.right != null && NodeArr[i].parent.left != null)
                            {
                                MoveUp(NodeArr[i]);
                            }
                        }
                    }
                } else
                {

                    if (IsMathOperator(resultStack.Last.Value))
                    {
                        if (CurNode != null && (CurNode.right != null || CurNode.left != null))
                        {
                            while (true)
                            {
                                if (CurNode.left == null)
                                {
                                    break;
                                }
                                MoveUp(CurNode);
                            }

                            NodeArr[CurNode.id].left = new Node(resultStack.Last.Value, i);
                            NodeArr[i] = NodeArr[CurNode.id].left;
                            NodeArr[i].parent = NodeArr[CurNode.id];
                            resultStack.RemoveLast();
                            CurNode = null;
                        }
                    }
                    else if (!IsMathOperator(resultStack.Last.Value))
                    {
                        if (CurNode != null && CurNode.left != null )
                        {
                            while (true)
                            {
                                if (CurNode.left == null)
                                {
                                    break;
                                }
                                MoveUp(CurNode);
                            }
                            NodeArr[CurNode.id].left = new Node(resultStack.Last.Value, i);
                            NodeArr[i] = NodeArr[CurNode.id].left;
                            NodeArr[i].parent = NodeArr[CurNode.id];
                            resultStack.RemoveLast();
                            //CurNode = null;
                        }
                    }
                    
                }
            }
        }

        public void MoveUp(Node node)
        {
            if (node.parent != null)
            {
                CurNode = node.parent;
            } else
            {
                CurNode = node;
            }
            
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program start\n");
            LexicalAnalyzer la = new LexicalAnalyzer();
            SyntaxAnalyzer sa;
            String codeText = la.GetCode();
            
            if (!la.CheckBrackets(codeText))
            {
                Console.WriteLine("Brackets error");
            }
            la.GetTokens(codeText);
            sa = new SyntaxAnalyzer(la.tokenList);
            //sa.MakeTree(codeText);
            Console.Write(sa.RecursiveTree(-1,"-"));
            LinkedList<Token> tlist = la.tokenList;            
            la.PrintList(tlist);

            Console.WriteLine("Program end\n");
        }
    }
}
