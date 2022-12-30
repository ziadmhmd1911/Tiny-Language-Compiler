﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Language
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        private bool CheckDataType(int InputPointer)
        {
            bool isInt = TokenStream[InputPointer].token_type == Token_Class.Int;
            bool isFloat = TokenStream[InputPointer].token_type == Token_Class.Float;
            bool isString = TokenStream[InputPointer].token_type == Token_Class.String;
            return (isInt || isFloat || isString);
        }
        private bool CheckStatments(int InputPointer)
        {
            bool isDecleration = CheckDataType(InputPointer);
            bool isWrite = TokenStream[InputPointer].token_type == Token_Class.Write;
            bool isRead = TokenStream[InputPointer].token_type == Token_Class.Read;
            bool isConditionOrFunctionCallOrAssignment = TokenStream[InputPointer].token_type == Token_Class.Idenifier;
            bool isIf = TokenStream[InputPointer].token_type == Token_Class.If;
            bool isRepeat = TokenStream[InputPointer].token_type == Token_Class.Repeat;
            return (isDecleration || isWrite || isRead || isConditionOrFunctionCallOrAssignment || isIf || isRepeat);
        }
        private bool CheckConditionOp(int InputPointer)
        {
            bool isLessThan = TokenStream[InputPointer].token_type == Token_Class.LessThanOp;
            bool isGreaterThan = TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp;
            bool isEqual = TokenStream[InputPointer].token_type == Token_Class.EqualOp;
            bool isNotEqual = TokenStream[InputPointer].token_type == Token_Class.NotEqualOp;
            return (isEqual || isGreaterThan || isLessThan || isNotEqual);
        }
        private bool CheckBooleanOp(int InputPointer)
        {
            bool isOr = TokenStream[InputPointer].token_type == Token_Class.OrOp;
            bool isAnd = TokenStream[InputPointer].token_type == Token_Class.AndOp;
            return (isAnd|| isOr);
        }
        private bool CheckTerm(int InputPointer)
        {
            bool isNumber = TokenStream[InputPointer].token_type == Token_Class.Constant;
            bool isIdentifier = TokenStream[InputPointer].token_type == Token_Class.Idenifier;

            return (isNumber || isIdentifier);
        }
        private bool CheckMathOp(int InputPointer)
        {
            bool isPlus = TokenStream[InputPointer].token_type == Token_Class.PlusOp;
            bool isMinus = TokenStream[InputPointer].token_type == Token_Class.MinusOp;
            return isPlus || isMinus;
        }
        private bool CheckMathOp2(int InputPointer)
        {
            bool isMult = TokenStream[InputPointer].token_type == Token_Class.MultiplyOp;
            bool isDivide = TokenStream[InputPointer].token_type == Token_Class.DivideOp;
            return isMult|| isDivide;
        }
        private bool CheckEquation(int InputPointer)
        {
            bool isLParanthesis = TokenStream[InputPointer].token_type == Token_Class.LParanthesis;
            return (isLParanthesis || CheckTerm(InputPointer));
        }
        private bool isExpression(int InputPointer)
        {
            bool isString = TokenStream[InputPointer].token_type == Token_Class.String;
            return (isString || CheckEquation(InputPointer));
        }
        private void printError(string Expected, int inputPointer = -1)
        {
            if (inputPointer == -1)
                inputPointer = InputPointer;
            if (inputPointer < TokenStream.Count)
                Errors.Error_List.Add("Parsing Error: We Expected "+ Expected + " But " +TokenStream[inputPointer].token_type.ToString() +"  found\r\n");
            else
                Errors.Error_List.Add("Parsing Error: We Expected "+ Expected + " But Nothing Found\r\n");
            InputPointer++;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(ProgramD());
            program.Children.Add(MainFunction());
            return program;
        }
        private Node ProgramD()
        {
            Node programD = new Node("Program'");
            if (InputPointer + 1 < TokenStream.Count && CheckDataType(InputPointer) && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                programD.Children.Add(FunctionStatement());
                programD.Children.Add(ProgramD());
                return programD;
            }
            else
                return null;
        }
        private Node FunctionStatement()
        {
            Node functionStatement = new Node("Function Statement");
            functionStatement.Children.Add(FunctionDeclaration());
            functionStatement.Children.Add(FunctionBody());
            return functionStatement;
        }
        private Node FunctionDeclaration()
        {
            Node functionDec = new Node("Function Declaration");
            functionDec.Children.Add(Datatype());
            functionDec.Children.Add(match(Token_Class.Idenifier));
            functionDec.Children.Add(match(Token_Class.LParanthesis));
            functionDec.Children.Add(Parameters());
            functionDec.Children.Add(match(Token_Class.RParanthesis));
            return functionDec;
        }
        private Node Parameters()
        {
            Node parameteres = new Node("Parameters");
            if (InputPointer < TokenStream.Count && CheckDataType(InputPointer))
            {
                parameteres.Children.Add(Parameter());
                parameteres.Children.Add(ParametersD());
                return parameteres;
            }
            else
                return null;
        }
        private Node ParametersD()
        {
            Node parameterD = new Node("Parameter'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameterD.Children.Add(match(Token_Class.Comma));
                parameterD.Children.Add(Parameter());
                parameterD.Children.Add(ParametersD());
                return parameterD;
            }
            else
                return null;
        }
        private Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }
        private Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            else
            {
                printError("Datatype");
            }
            return datatype;
        }
        private Node MainFunction()
        {
            Node main = new Node("Main");
            main.Children.Add(Datatype());
            main.Children.Add(match(Token_Class.Main));
            main.Children.Add(match(Token_Class.LParanthesis));
            main.Children.Add(match(Token_Class.RParanthesis));
            main.Children.Add(FunctionBody());
            return main;
        }
        private Node FunctionBody()
        {
            Node functionBody = new Node("Function Body");
            functionBody.Children.Add(match(Token_Class.LCurlyBracket));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(ReturnStatement());
            functionBody.Children.Add(match(Token_Class.RCurlyBracket));
            return functionBody;
        }
        private Node ReturnStatement()
        {
            Node returnStatement = new Node("Return Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            return returnStatement;
        }
        private Node Expression()
        {
            Node exp = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                exp.Children.Add(match(Token_Class.String));
            }
            else if (InputPointer < TokenStream.Count && CheckEquation(InputPointer))
            {
                exp.Children.Add(Equation());
            }
            else
                printError("Expression");
            return exp;
        }
        private Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Factor());
            equation.Children.Add(EquationD());
            return equation;
        }
        private Node Factor()
        {
            Node factor = new Node("Factor");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                factor.Children.Add(match(Token_Class.LParanthesis));
                factor.Children.Add(Equation());
                factor.Children.Add(match(Token_Class.RParanthesis));
                factor.Children.Add(FactorD());
            }
            else if (InputPointer < TokenStream.Count && CheckTerm(InputPointer))
            {
                factor.Children.Add(Term());
                factor.Children.Add(FactorD());
            }
            else
                printError("Factor");
            return factor;
        }
        private Node Term()
        {
            Node term = new Node("Term");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                term.Children.Add(match(Token_Class.Constant));
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    term.Children.Add(FunctionCall());
                }
                else
                    term.Children.Add(match(Token_Class.Idenifier));
            }
            else
                printError("Term");
            return term;
        }
        private Node FactorD()
        {
            Node factD = new Node("Factor'");
            if (InputPointer < TokenStream.Count && CheckMathOp2(InputPointer))
            {
                factD.Children.Add(MultOp());
                factD.Children.Add(Equation());
                factD.Children.Add(FactorD());
                return factD;
            }
            else
                return null;
        }
        private Node EquationD()
        {
            Node eqD = new Node("Equation'");
            if (InputPointer < TokenStream.Count && CheckMathOp(InputPointer))
            {
                eqD.Children.Add(AddOp());
                eqD.Children.Add(Factor());
                eqD.Children.Add(EquationD());
                return eqD;
            }
            else
                return null;
        }
        private Node AddOp()
        {
            Node addOp = new Node("Add Operator");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                addOp.Children.Add(match(Token_Class.PlusOp));
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                addOp.Children.Add(match(Token_Class.MinusOp));
            }
            else
                printError("Addition Operator");
            return addOp;
        }
        private Node MultOp()
        {
            Node multOp = new Node("Mult Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                multOp.Children.Add(match(Token_Class.MultiplyOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                multOp.Children.Add(match(Token_Class.DivideOp));
            }
            else
                printError("Multiply or Divide Operator");
            return multOp;
        }
        private Node Statements()
        {
            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count && CheckStatments(InputPointer))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(StatementsD());
                return statements;
            }
            else
                return null;
        }
        private Node StatementsD()
        {
            Node statementD = new Node("Statements'");
            if (InputPointer < TokenStream.Count && CheckStatments(InputPointer))
            {
                statementD.Children.Add(Statement());
                statementD.Children.Add(StatementsD());
                return statementD;
            }
            else
                return null;
        }
        private Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                // Assignment
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
                {
                    statement.Children.Add(AssignmentStatment());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                //Condition
                else if (InputPointer + 1 < TokenStream.Count && CheckConditionOp(InputPointer + 1))
                {
                    statement.Children.Add(ConditionStatement());
                }
                //Function Call
                else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    statement.Children.Add(FunctionCall());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else
                    printError("Assignment or Condition or Function", InputPointer + 1);
            }
            // Declaration
            else if (InputPointer < TokenStream.Count && CheckDataType(InputPointer))
            {
                statement.Children.Add(DeclarationStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(WriteStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(IfStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                statement.Children.Add(RepeatStatement());
            }
            else
                printError("Statement");
            return statement;
        }
        private Node RepeatStatement()
        {
            Node repeatStatement = new Node("Repeat Statement");
            repeatStatement.Children.Add(match(Token_Class.Repeat));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Token_Class.Until));
            repeatStatement.Children.Add(ConditionStatement());
            return repeatStatement;
        }
        private Node IfStatement()
        {
            Node ifStatement = new Node("If Statement");
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(ConditionStatement());
            ifStatement.Children.Add(match(Token_Class.Then));
            ifStatement.Children.Add(StatementsOrReturn());
            ifStatement.Children.Add(RestIf());

            return ifStatement;
        }
        private Node StatementsOrReturn()
        {
            Node statementsOrReturn = new Node("Statements Or Return");
            if (InputPointer < TokenStream.Count && CheckStatments(InputPointer))
            {
                statementsOrReturn.Children.Add(Statements());
                statementsOrReturn.Children.Add(StatementsOrReturn());
                return statementsOrReturn;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                statementsOrReturn.Children.Add(ReturnStatement());
                return statementsOrReturn;
            }
            else
                return null;
        }
        private Node RestIf()
        {
            Node restIf = new Node("Rest If");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Elseif)
            {
                restIf.Children.Add(ElseIfStatement());
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                restIf.Children.Add(ElseStatement());
            }
            else if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
            {
                restIf.Children.Add(match(Token_Class.End));
            }
            else
                printError("End");
            return restIf;
        }
        private Node ElseStatement()
        {
            Node elseStatement = new Node("Else Statement");
            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(StatementsOrReturn());
            elseStatement.Children.Add(match(Token_Class.End));
            return elseStatement;
        }
        private Node ElseIfStatement()
        {
            Node elseIfStatement = new Node("Else If Statement");
            elseIfStatement.Children.Add(match(Token_Class.Elseif));
            elseIfStatement.Children.Add(ConditionStatement());
            elseIfStatement.Children.Add(match(Token_Class.Then));
            elseIfStatement.Children.Add(StatementsOrReturn());
            elseIfStatement.Children.Add(RestIf());
            return elseIfStatement;
        }
        private Node ReadStatement()
        {
            Node readStatement = new Node("Read Statement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon));
            return readStatement;
        }
        private Node WriteStatement()
        {
            Node writeStatement = new Node("Write Statement");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(WriteD());
            writeStatement.Children.Add(match(Token_Class.Semicolon));
            return writeStatement;
        }
        private Node WriteD()
        {
            Node writeD = new Node("Write'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writeD.Children.Add(match(Token_Class.Endl));
            }
            else if (InputPointer < TokenStream.Count && isExpression(InputPointer))
            { 
                writeD.Children.Add(Expression());
            }
            else
                printError("Write Statement");
            return writeD;
        }
        private Node AssignmentStatment()
        {
            Node assign = new Node("Assignment Statment");
            assign.Children.Add(match(Token_Class.Idenifier));
            assign.Children.Add(match(Token_Class.AssignmentOp));
            assign.Children.Add(Expression());
            return assign;
        }
        private Node DeclarationStatement()
        {
            Node declarationStatement = new Node("Declaration Statement");
            declarationStatement.Children.Add(Datatype());
            declarationStatement.Children.Add(Identifiers());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }
        private Node Identifiers()
        {
            Node ids = new Node("IDs");
            if(InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.Comma || TokenStream[InputPointer + 1].token_type == Token_Class.Semicolon))
            {
                ids.Children.Add(match(Token_Class.Idenifier));
                ids.Children.Add(IdsD());
            }
            else if(InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
            {
                ids.Children.Add(AssignmentStatment());
                ids.Children.Add(IdsD());
            }
            else
                printError("Identifiers");
            return ids;
        }
        private Node IdsD()
        {
            Node idsD = new Node("IDs'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idsD.Children.Add(match(Token_Class.Comma));
                idsD.Children.Add(Identifiers());
                return idsD;
            }
            else
                return null;
        }
        private Node ConditionStatement()
        {
            Node conditionStatement = new Node("Condition Statement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(Conditions());
            return conditionStatement;
        }
        private Node Conditions()
        {
            Node conditions = new Node("Conditions");
            if (InputPointer < TokenStream.Count && CheckBooleanOp(InputPointer))
            {
                conditions.Children.Add(BooleanOp());
                conditions.Children.Add(Condition());
                conditions.Children.Add(Conditions());
                return conditions;
            }
            else
                return null;
        }
        private Node BooleanOp()
        {
            Node boolOp = new Node("Boolean Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                boolOp.Children.Add(match(Token_Class.AndOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                boolOp.Children.Add(match(Token_Class.OrOp));
            }
            else
                printError("Boolean Operator");
            return boolOp;
        }
        private Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(ConditionOp());
            condition.Children.Add(Term());

            return condition;
        }
        private Node ConditionOp()
        {
            Node condOp = new Node("Condition Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condOp.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condOp.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                condOp.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condOp.Children.Add(match(Token_Class.NotEqualOp));
            }
            else
                printError("Condition Operator");
            return condOp;
        }
        private Node FunctionCall()
        {
            Node funcCall = new Node("Function Call");
            funcCall.Children.Add(match(Token_Class.Idenifier));
            funcCall.Children.Add(match(Token_Class.LParanthesis));
            funcCall.Children.Add(Arguments());
            funcCall.Children.Add(match(Token_Class.RParanthesis));
            return funcCall;
        }
        private Node Arguments()
        {
            Node args = new Node("Identifiers");
            if (InputPointer < TokenStream.Count && isExpression(InputPointer))
            {
                args.Children.Add(Expression());
                args.Children.Add(ArgumentsD());
                return args;
            }
            else
                return null;
        }
        private Node ArgumentsD()
        {
            Node idD = new Node("Identifier'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idD.Children.Add(match(Token_Class.Comma));
                idD.Children.Add(Expression());
                idD.Children.Add(ArgumentsD());
                return idD;
            }
            else
                return null;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}