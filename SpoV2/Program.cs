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
            for (int i = 0; i < n; i++)
            {                
                tkn = tkns.Last.Value;
                tkns.RemoveLast();
                Console.WriteLine($"Name: {tkn.name}, Type: {tkn.type}, Value: {tkn.val}\n");
            }
        }

        public bool IsKeyWord(string text)
        {
            string[] keyWords = { "List", "int", "new", "foreach", "in", "if", "else" };

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
            string[] seporators = { ";", "(", ")", " ", "\n", "\t", ",", "{", "}", "\r", "[", "]", "/" };
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

        public void GetTokens(String code)
        {
            String subString = "";       
            
            char ch = '0';

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
                                        PushToken(variableValue, "Number", "null");
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
                                GetVarArg:
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
                    }
                }
            }
        EndGetToken:;
        }

    }

    class SyntaxAnalyzer
    {
        public LinkedList<Token> tokens;
    }

    class BinaryTree<T> where T : IComparable<T>
    {
        private BinaryTree<T> parent, left, right;
        private String name, type;
        private List<String> listForPrint = new List<String>();

        public BinaryTree(String name, String type, BinaryTree<T> parent)
        {
            this.name = name;
            this.type = type;
            this.parent = parent;
        }

        public void add(String name, String type )
        {
            if (name.CompareTo(this.name) < 0)
            {
                if (this.left == null)
                {
                    this.left = new BinaryTree<T>(name, type, this);
                }
                else if (this.left != null)
                    this.left.add(name, type);
            }
            else
            {
                if (this.right == null)
                {
                    this.right = new BinaryTree<T>(name, type, this);
                }
                else if (this.right != null)
                    this.right.add(name, type);
            }
        }

        private BinaryTree<T> _search(BinaryTree<T> tree, String name)
        {
            if (tree == null) return null;
            switch (name.CompareTo(tree.name))
            {
                case 1: return _search(tree.right, name);
                case -1: return _search(tree.left, name);
                case 0: return tree;
                default: return null;
            }
        }

        public BinaryTree<T> search(String name)
        {
            return _search(this, name);
        }

        public bool remove(String name)
        {
            //Проверяем, существует ли данный узел
            BinaryTree<T> tree = search(name);
            if (tree == null)
            {
                //Если узла не существует, вернем false
                return false;
            }
            BinaryTree<T> curTree;

            //Если удаляем корень
            if (tree == this)
            {
                if (tree.right != null)
                {
                    curTree = tree.right;
                }
                else curTree = tree.left;

                while (curTree.left != null)
                {
                    curTree = curTree.left;
                }
                String temp = curTree.name;
                this.remove(temp);
                tree.name = temp;

                return true;
            }

            //Удаление листьев
            if (tree.left == null && tree.right == null && tree.parent != null)
            {
                if (tree == tree.parent.left)
                    tree.parent.left = null;
                else
                {
                    tree.parent.right = null;
                }
                return true;
            }

            //Удаление узла, имеющего левое поддерево, но не имеющее правого поддерева
            if (tree.left != null && tree.right == null)
            {
                //Меняем родителя
                tree.left.parent = tree.parent;
                if (tree == tree.parent.left)
                {
                    tree.parent.left = tree.left;
                }
                else if (tree == tree.parent.right)
                {
                    tree.parent.right = tree.left;
                }
                return true;
            }

            //Удаление узла, имеющего правое поддерево, но не имеющее левого поддерева
            if (tree.left == null && tree.right != null)
            {
                //Меняем родителя
                tree.right.parent = tree.parent;
                if (tree == tree.parent.left)
                {
                    tree.parent.left = tree.right;
                }
                else if (tree == tree.parent.right)
                {
                    tree.parent.right = tree.right;
                }
                return true;
            }

            //Удаляем узел, имеющий поддеревья с обеих сторон
            if (tree.right != null && tree.left != null)
            {
                curTree = tree.right;

                while (curTree.left != null)
                {
                    curTree = curTree.left;
                }

                //Если самый левый элемент является первым потомком
                if (curTree.parent == tree)
                {
                    curTree.left = tree.left;
                    tree.left.parent = curTree;
                    curTree.parent = tree.parent;
                    if (tree == tree.parent.left)
                    {
                        tree.parent.left = curTree;
                    }
                    else if (tree == tree.parent.right)
                    {
                        tree.parent.right = curTree;
                    }
                    return true;
                }
                //Если самый левый элемент НЕ является первым потомком
                else
                {
                    if (curTree.right != null)
                    {
                        curTree.right.parent = curTree.parent;
                    }
                    curTree.parent.left = curTree.right;
                    curTree.right = tree.right;
                    curTree.left = tree.left;
                    tree.left.parent = curTree;
                    tree.right.parent = curTree;
                    curTree.parent = tree.parent;
                    if (tree == tree.parent.left)
                    {
                        tree.parent.left = curTree;
                    }
                    else if (tree == tree.parent.right)
                    {
                        tree.parent.right = curTree;
                    }

                    return true;
                }
            }
            return false;
        }

        private void _print(BinaryTree<T> node)
        {
            if (node == null) return;
            _print(node.left);
            listForPrint.Add(node.name);
            Console.Write(node + " ");
            if (node.right != null)
                _print(node.right);
        }

        public void print()
        {
            listForPrint.Clear();
            _print(this);
            Console.WriteLine();
        }

        //public override string ToString()
        //{
        //    return val.ToString();
        //}

    }


    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyzer la = new LexicalAnalyzer();
            
            String codeText = la.GetCode();

            if (!la.CheckBrackets(codeText))
            {
                Console.WriteLine("Brackets error");
            }

            la.GetTokens(codeText);
            LinkedList<Token> tlist = la.tokenList;
            tlist.Reverse();
            la.PrintList(tlist);

            Console.WriteLine("");
        }
    }
}
