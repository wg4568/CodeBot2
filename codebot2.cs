using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CodeBot2 {
	public static void Main(string[] args) {
		
		string input = "";
		string output = "";
		string raw_input = "";
		string raw_output = "";
		
		if (args.Length == 0) {
			Console.WriteLine("Usage: codebot2.exe INPUTFILE OUTPUTFILE");
		} else if (args.Length == 1) {
			input = args[0];
			output = Path.GetFileNameWithoutExtension(input) + ".c";
		} else if (args.Length == 2) {
			input = args[0];
			output = args[1];
		}
		
		if (input != "") {
			raw_input = File.ReadAllText(input);
		}

		if (output == "") {
			output = "untitled.c";
		}

		if (args.Length != 0) {
			Parser parser = new Parser();
	
			parser.Parse(raw_input);
			parser.Compile();
			
			raw_output = parser.code;
			
			Console.WriteLine(" >> Writing code to " + output);
			Console.WriteLine(raw_output);
			File.WriteAllText(output, raw_output);
		}
	}
}

enum TokenKind {
	Token,
	String,
	Number,
	Keyword,
	Open,
	Close,
	Comma,
	Semicolon,
	Constant
}

class Parser {
	public string raw;
	public string code;
	public List<Token> tokens;
	public List<Statement> statements;

	public List<string> code_motors;
	public List<string> code_constants;
	public List<string> code_triggers;

	public Parser() {
		tokens = new List<Token>();
		statements = new List<Statement>();
		
		code_motors = new List<string>();
		code_constants = new List<string>();
		code_triggers = new List<string>();
	}
	
	public override string ToString() {
		string str = "";
		for (int i = 0; i < statements.Count; i++) {
			if (i != 0) { str += "\n"; }
			str += statements[i].ToString();
		}
		// return str + "\n\n===================\n" + code;
		return code;
	}
	
	public void Parse(string raw_string) {
		raw += raw_string;

		Token current = new Token(TokenKind.Token, "");
		Statement current_statement = new Statement(new List<Token>());

		for (int i = 0; i < raw_string.Length; i++) {
			var character = raw_string[i].ToString();

			if (current.type == TokenKind.Number) {
				if (!(Regex.IsMatch(character, @"^\d+$") || character == ".")) {
					current_statement.Append(current);
					current = new Token(TokenKind.Token, "");
				}
			}

			if (current.type == TokenKind.Keyword) {
				if (!(Regex.IsMatch(character, @"^[a-zA-Z]+$") || character == "_")) {
					current_statement.Append(current);
					current = new Token(TokenKind.Token, "");
				}
			}

			if (current.type == TokenKind.Constant) {
				if (!(Regex.IsMatch(character, @"^[a-zA-Z]+$") || character == "_")) {
					current_statement.Append(current);
					current = new Token(TokenKind.Token, "");
				}
			}

			if (character == "'" || character == "\"") {
				if (current.type == TokenKind.String) {
					current_statement.Append(current);
					current = new Token(TokenKind.Token, "");
				} else {
					current.SetKind(TokenKind.String);
				}
			} else if (current.type == TokenKind.String) {
				current.Append(character);
			} else if (character == "{") {
				current.SetKind(TokenKind.Open);
				current_statement.Append(current);
				current = new Token(TokenKind.Token, "");
			} else if (character == "}") {
				current.SetKind(TokenKind.Close);
				current_statement.Append(current);
				current = new Token(TokenKind.Token, "");
			} else if (character == ",") {
				current.SetKind(TokenKind.Comma);
				current_statement.Append(current);
				current = new Token(TokenKind.Token, "");
			} else if (character == "$") {
				current.SetKind(TokenKind.Constant);
			} else if (character == ";") {
				current.SetKind(TokenKind.Semicolon);
				current_statement.Parse();
				statements.Add(current_statement);
				current_statement = new Statement(new List<Token>());
				// current_statement.Append(current);
				current = new Token(TokenKind.Token, "");
			} else if (Regex.IsMatch(character, @"^\d+$") || character == "-") {
				if (current.type == TokenKind.Number) {
					current.Append(character);
				} else {
					current.SetKind(TokenKind.Number);
					current.Append(character);
				}
			} else if (character == ".") {
				if (current.type == TokenKind.Number) {
					current.Append(character);
				}
			} else if (Regex.IsMatch(character, @"^[a-zA-Z]+$")) {
				if (current.type == TokenKind.Keyword || current.type == TokenKind.Constant) {
					current.Append(character);
				} else {
					current.SetKind(TokenKind.Keyword);
					current.Append(character);
				}
			} else if (Regex.IsMatch(character, @"^[a-zA-Z]+$") || character == "_") {
				if (current.type == TokenKind.Keyword) {
					current.Append(character);
				} else if (current.type == TokenKind.Constant) {
					current.Append(character);
				}
			}
		}

		if (current_statement.tokens.Count != 0) {
			current_statement.Parse();
			statements.Add(current_statement);
		}
	}
	
	public void Compile() {
		string pragmas = "";
		string header = "/* Created using CodeBot2 - https://github.com/wg4568/CodeBot2 */\n";
		string input_header = "";
		string triggers = "";
		string main_start = "";
		string main_mid = "";
		string main_end = "";
		string auton = "";
		
		for (int i = 0; i < statements.Count; i++) {
			Statement statement = statements[i];
			
			if (statement.keywords[0].raw == "define") {
				if (statement.keywords[1].raw == "motor") {
					string type = "normal";
					string name = "MOTOR_" + statement.keywords[2].raw;
					bool reversed = false;
					int port = 1;
					
					foreach (var phrase in statement.phrases) {
						if (phrase.keyword.raw == "type") {
							type = phrase.arguments[0].raw;
						} else if (phrase.keyword.raw == "port") {
							port = Int32.Parse(phrase.arguments[0].raw);
						} else if (phrase.keyword.raw == "reversed") {
							reversed = phrase.arguments[0].raw == "true";
						}
					}

					pragmas += formatMotor(port, name, type, reversed);
					code_motors.Add(name);
				} else if (statement.keywords[1].raw == "constant") {
					string name = "CONST_" + statement.keywords[2].raw;
					string value = "0.0";
					
					foreach (var phrase in statement.phrases) {
						if (phrase.keyword.raw == "value") {
							value = phrase.arguments[0].raw;
						}
					}
					
					header += "float " + name + " = " + value + ";\n";
					
					code_constants.Add(name);
				} else if (statement.keywords[1].raw == "input") {
					string name = statement.keywords[2].raw;
					string type = "analog";
					string port = "1";

					foreach (var phrase in statement.phrases) {
						if (phrase.keyword.raw == "type") {
							type = phrase.arguments[0].raw;
						} else if (phrase.keyword.raw == "port") {
							port = "";
							foreach (var arg in phrase.arguments) {
								port += arg.raw;
							}
						}
					}

					string prefix = type == "digital" ? "Btn" : "Ch";
					string full_port = prefix + port;

					input_header += "float INPUT_OLD_" + name + " = 0.0;\n";
					input_header += "float INPUT_" + name + " = 0.0;\n";
					
					main_start += "INPUT_" + name + " = vexRT[" + full_port + "];\n";
					main_end += "INPUT_OLD_" + name + " = INPUT_" + name + ";\n";
				}
			} else if (statement.keywords[0].raw == "trigger") {
				string mode = statement.keywords[1].raw;
				string name = statement.keywords[2].raw;

				string oname = "INPUT_OLD_" + name;
				string cname = "INPUT_" + name;

				string tcode = "";
				
				foreach (var phrase in statement.phrases) {
					if (phrase.keyword.raw == "set") {
						string mname = phrase.arguments[0].raw;
						string valcode = "";
						
						for (int z = 1; z < phrase.arguments.Count; z++) {
							Token arg = phrase.arguments[z];
	
							if (arg.type == TokenKind.Constant) {
								if (arg.raw == "value") {
									valcode += cname;
								} else {
									valcode += "CONST_" + arg.raw;
								}
							} else {
								valcode += arg.raw;
							}
						}
						
						tcode += "motor[MOTOR_" + mname + "] = " + valcode + ";\n";
					}
				}
				
				string logic = "";
				
				if (mode == "change") {
					logic = "(" + oname + " != " + cname + ")";
				} else if (mode == "down") {
					logic = "(" + oname + " == 0 && " + cname + " == 1)";
				} else if (mode == "up") {
					logic = "(" + oname + " == 1 && " + cname + " == 0)";
				} else if (mode == "held") {
					logic = "(" + oname + " == 1)";
				} else if (mode == "always") {
					logic = "(true)";
				}

				string function = "void TRIGGER_" + mode + "_" + name + "() {\n"
						+ "if " + logic + " {\n"
						+ tcode + "}\n"
						+ "}\n";
				
				input_header += function;
				
				main_mid += "TRIGGER_" + mode + "_" + name + "();\n";
			}
		}
		
		string main_block = "task main() { while (true) {\n"
								+ main_start
								+ main_mid
								+ main_end + "}}";

		code = pragmas + header + input_header + triggers + main_block;
	}
	
	public string formatMotor(int port, string name, string type, bool reversed) {
		string raw = "#pragma config(Motor,port{0},{1},{2},openLoop{3})\n";
		string reversed_string = "";
		string type_string = "";
		
		if (type == "normal") { type_string = "tmotorVex393_HBridge"; }
		if (reversed) { reversed_string = ",reversed"; }

		return String.Format(raw, port, name, type_string, reversed_string);
	}
}

class Token {
	public string raw;
	public TokenKind type;

	public Token(TokenKind kind, string value) {
		type = kind;
		raw = value;
	}
	
	public void Append(string stuff) {
		raw += stuff;
	}
	
	public void SetKind(TokenKind kind) {
		type = kind;
	}

	public override string ToString() {
		return String.Format("{0}({1})", type, raw);
	}
}

class Phrase {
	public List<Token> tokens;
	public List<Token> arguments;
	public Token keyword;
	
	public Phrase(List<Token> raw_tokens) {
		tokens = new List<Token>();
		arguments = new List<Token>();
		
		tokens = raw_tokens;
		
		keyword = tokens[0];
		for (int i = 1; i < tokens.Count; i++) {
			arguments.Add(tokens[i]);
		}
	}
	
	public override string ToString() {
		return String.Format("Phrase({0}, {1})", keyword.raw, string.Join(", ", arguments));
	}
}

class Statement {
	public List<Token> tokens;
	public List<Token> keywords;
	public List<Phrase> phrases;
	
	public Statement(List<Token> raw_tokens) {
		keywords = new List<Token>();
		phrases = new List<Phrase>();
		
		tokens = raw_tokens;
	}
	
	public override string ToString() {
		string s_args = string.Join(", ", keywords);
		string s_phrases = string.Join("\n  ", phrases);
		return String.Format("Statement(\n  {0}\n  {1}\n)", s_args, s_phrases);
	}
	
	public void Append(Token tok) {
		tokens.Add(tok);
	}
	
	public void Parse() {
		bool in_keywords = true;
		bool in_phrase = false;

		List<Token> current = new List<Token>();
		
		for (int i = 0; i < tokens.Count; i++) {
			Token tok = tokens[i];
			
			if (in_keywords) {
				if (tok.type == TokenKind.Open) {
					in_keywords = false;
				} else {
					keywords.Add(tok);
				}
			} else {
				if (tok.type == TokenKind.Comma || tok.type == TokenKind.Close) {
					if (current.Count != 0) {
						Phrase phrase = new Phrase(current);
						phrases.Add(phrase);
						current = new List<Token>();
					}
				} else {
					in_phrase = true;
					current.Add(tok);
				}
			}
		}

		if (current.Count != 0) {
			phrases.Add(new Phrase(current));
		}
	}
}