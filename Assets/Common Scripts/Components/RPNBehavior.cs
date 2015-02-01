using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPNBehavior : MonoBehaviour {
	public bool running=false;
	public bool looping=false;
	public int CyclesPerFrame=10;
	string console="";
	public string rawCode="";
	
	List<string> splitCode=new List<string>();
	Dictionary<string,int> jumpPoints=new Dictionary<string, int>();
	
	int point=0;
	
	public delegate bool Operator();
	Dictionary<string, Operator> operators=new Dictionary<string, Operator>();
	Dictionary<string,string> ram=new Dictionary<string, string>();
	Stack<string> stack=new Stack<string>();
	
	public void Start(){LoadCode(rawCode);ResetOperators();}
	//Operator API
	virtual public void ResetOperators(){
		operators.Clear();
		RegisterOperator("reset",Reset);
		RegisterOperator("clear",Clear);
		
		RegisterOperator("set",Set);
		RegisterOperator("get",Get);
		
		RegisterOperator("goto",Op_Goto);
		RegisterOperator("if",If);
		
		RegisterOperator("+",Add);
		RegisterOperator("-",Sub);
		RegisterOperator("/",Div);
		RegisterOperator("*",Mul);
		
		RegisterOperator(">",UnimplementedFunction);
		RegisterOperator(">=",UnimplementedFunction);
		RegisterOperator("<",UnimplementedFunction);
		RegisterOperator("<=",UnimplementedFunction);
		RegisterOperator("=",UnimplementedFunction);
		RegisterOperator("!=",UnimplementedFunction);
		
		RegisterOperator("print",Print);
		
		//extension
		RegisterOperator("move",Move);
		RegisterOperator("look",Look);
		RegisterOperator("lookat",OpLookAt);
	}
	public void RegisterOperator(string key, Operator value){
		operators.Add (key,value);
		Print ("Registered Operator \"" + key + "\"");
	}
	//File IO
	public void LoadFile(string fileName){
		LoadCode(System.IO.File.ReadAllText(fileName));
	}
	public void SaveFile(string fileName){
		System.IO.File.WriteAllText(fileName,rawCode);
	}
	public string GetCode(){return rawCode;}
	public void LoadCode(string newCode){
		rawCode=newCode;
		splitCode=new List<string>(rawCode.Split(new char[] {' ','\n','\r'}));
		jumpPoints.Clear();
		int i=0;
		foreach(string operation in splitCode){
			if(operation.StartsWith("@")){
				jumpPoints.Add(operation,i);
				operation.Remove(0);
				ram[operation]=i.ToString();
				Print ("Registered Jump Point \"" + operation + "\" at " + i);
			}
			i++;
		}
	}
	public bool Execute(string code){
		string[] split=code.Split ();
		foreach(string current in split){
			if(!current.StartsWith("@")){
				if(operators.ContainsKey(current))
					operators[current]();
				else 
					stack.Push(current);
			}
		}
		return true;
	}
	public bool Step(int cycles){if(cycles>=1 && Step()){Step(cycles-1);return true;}else return false;}
	private bool pointProgressed=false;
	public bool Step(){
		pointProgressed=false;
		if(point>=splitCode.Count){
			if(looping==true)
				point=0;
			else
				return false;
		}
		string current=splitCode[point];
		Execute(current);
		if(!pointProgressed)
			point++;
		return true;
	}
	public void FixedUpdate(){
		if(running){
			Step (CyclesPerFrame);
		}
	}
	
	public void Run(){running=true;}
	public void Stop(){running=false;}
	public void Goto(int newPoint){point=newPoint;}
	private void Print(string s){console=s;Debug.Log(s);}
	public int GetPoint(){return point;}
	public bool IsRunning(){return running;}
	public string GetConsole(){return console;}
	//Error Messages
	private bool StackUnderflow(){Print ("STACK-UNDERFLOW @" + point);return false;}
	private bool UnimplementedFunction(){Print ("UNIMPLEMENTED FUNCTION @" + point);return false;}
	//Ram Operations
	public string[] GetRAMKeys(){
		string[] keys=new string[ram.Keys.Count];ram.Keys.CopyTo (keys,0);
		return keys;
	}
	public void Set(string key, string value){
		if(ram.ContainsKey(key))
			ram.Remove(key);
		ram.Add(key,value);
	}
	public string Get(string key){
		if(!ram.ContainsKey(key))
			return "0";
		else 
			return ram[key];
	}
	//Stack Operations
	public string[] GetStack(){
		return stack.ToArray();
	}
	public void Push(string value){
		stack.Push(value);
	}
	public string Pop(){
		return stack.Pop ();
	}
	public string Peek(){
		return stack.Peek ();
	}
	public bool Swap(){
		if(stack.Count<2) return StackUnderflow();
		string a,b;
		a=Pop ();
		b=Pop ();
		Push (a);
		Push (b);
		return true;
	}
	
	//Internal Operations
	public bool Reset(){
		Print ("RPN Reset");
		point=0;
		ram.Clear();
		stack.Clear();
		return true;
	}
	public bool Clear(){
		stack.Clear();
		return true;
	}
	private bool Set(){
		if(stack.Count<2) return StackUnderflow();
		
		Set(Pop(),Pop());//magnitude
		return true;
	}
	private bool Get(){
		if(stack.Count<1) return StackUnderflow();
		
		Push(Get(Pop()));
		return true;
	}
	
	private bool Op_Goto(){
		if(stack.Count<1) return StackUnderflow();
		string s=Pop ();
		int n;int.TryParse(s,out n);
		if(n==0)
			int.TryParse(Get("@"+s),out n);
		Goto(n);
		pointProgressed=true;
		return true;
	}
	private bool If(){return UnimplementedFunction();}
	
	private bool Add(){
		if(stack.Count<2) return StackUnderflow();
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		Push ((a+b).ToString());
		return true;
	}
	private bool Sub(){
		if(stack.Count<2) return StackUnderflow();
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		Push ((a-b).ToString());
		return true;
	}
	private bool Div(){
		if(stack.Count<2) return StackUnderflow();
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		Push ((a/b).ToString());
		return true;
	}
	private bool Mul(){
		if(stack.Count<2) return StackUnderflow();
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		Push ((a*b).ToString());
		return true;
	}
	
	private bool Print(){
		if(stack.Count<1) return StackUnderflow();
		
		int length;int.TryParse(Peek (),out length);
		if(length==0){console="";return true;}
		if(stack.Count<1+length) return StackUnderflow();
		
		Pop ();
		
		string s="";
		for(int i=0;i<length;i++){
			s=Pop()+" "+s;
		}
		Print (s);
		return true;
	}
	BaseCharacterController character{
		get{
			return GetComponent<BaseCharacterController>();
		}
	}
	
	private bool Move(){
		if(stack.Count<2) return StackUnderflow();
		
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		 
		character.moveLocal=true;
		character.moveDirection=new Vector3(b,0,a);
		
		return true;
	}
	private bool Look(){
		if(stack.Count<1) return StackUnderflow();
		
		float a;
		float.TryParse(Pop (), out a);
		
		character.lookAngle=new Vector2(0,a);
		
		return true;
	}
	private bool OpLookAt(){
		if(stack.Count<2) return StackUnderflow();
		
		float a,b;
		float.TryParse(Pop (),out a);
		float.TryParse(Pop (),out b);
		
		transform.LookAt (new Vector3(a,0,b));
		character.lookAngle=new Vector2(0,transform.rotation.eulerAngles.y);
		
		return true;
	}
}
