using System;
using System.Diagnostics;
using System.Text;
using System.Reflection;

namespace LaytonControlLibrary.Diagnostics 
{
	public class CustomTraceListener : TextWriterTraceListener 
	{
		private static TraceSwitch traceSwitch = new TraceSwitch("AppTraceLevel", null);
		
		// for our constructors, explicitly call the base class constructor.
		public CustomTraceListener( System.IO.Stream stream, string name ) :
			base(stream, name) { }
		public CustomTraceListener( System.IO.Stream stream) : 
			base(stream) { }
		public CustomTraceListener( string fileName, string name ) : 
			base(fileName, name) {	}
		public CustomTraceListener( string fileName ) : 
			base(fileName) { }
		public CustomTraceListener( System.IO.TextWriter writer, string name ) : 
			base(writer, name) { }
		public CustomTraceListener( System.IO.TextWriter writer ) : 
			base(writer) { }

	
		public override void Write( string message ) 
		{	
			
			base.Write( getPreambleMessage() + message );
		}

		public override void WriteLine( string message ) 
		{
			if (traceSwitch.TraceInfo)
				base.WriteLine( getPreambleMessage() + message );
		} 

		
		[System.Runtime.CompilerServices.MethodImpl
			(
			 System.Runtime.CompilerServices.MethodImplOptions.NoInlining )]
		private string getPreambleMessage() 
		{
			StringBuilder preamble = new StringBuilder();
			
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame;
			MethodBase stackFrameMethod;

			int frameCount = 0;
			string typeName;
			do {
				frameCount++;
				stackFrame	= stackTrace.GetFrame(frameCount);
				stackFrameMethod = stackFrame.GetMethod();
				typeName = stackFrameMethod.ReflectedType.FullName;
			} while ( typeName.StartsWith("System") || typeName.EndsWith("CustomTraceListener") );
			
			//log DateTime, Namespace, Class and Method Name
			preamble.Append(DateTime.Now.ToString());
			preamble.Append(": ");
			preamble.Append(typeName);
			preamble.Append(".");
			preamble.Append(stackFrameMethod.Name);
			preamble.Append("( ");
		
			// log parameter types and names
			ParameterInfo[] parameters = stackFrameMethod.GetParameters();
			int parameterIndex = 0;
			while( parameterIndex < parameters.Length ) {
				preamble.Append(parameters[parameterIndex].ParameterType.Name);
				preamble.Append(" ");
				preamble.Append(parameters[parameterIndex].Name);
				parameterIndex++;
				if (parameterIndex != parameters.Length ) preamble.Append(", ");
			}
				
			preamble.Append(" ): ");

			return preamble.ToString();
		
		}
	}
}
