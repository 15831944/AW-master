using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.Common.Controls
{
	public class ProgressDetails
	{
		public enum eState { initialize, trying, success, failure }

		protected eState _State;
		protected string _Text;
		protected int _MaximumSteps;
		protected object _Tag;

		public ProgressDetails()
		{
			_State = eState.initialize;
			_Text = "";
			_MaximumSteps = 0;
		}

		public ProgressDetails(eState state, string strText)
		{
			_State = state;
			_Text = strText;
			_MaximumSteps = 0;
			_Tag = null;
		}

		public ProgressDetails(int nMaximumSteps)
			: base()
		{
			_MaximumSteps = nMaximumSteps;
		}

		public Object Tag
		{
			get { return _Tag; }
			set { _Tag = value; }
		}

		public int MaximumSteps
		{ get { return _MaximumSteps; } set { _MaximumSteps = value; } }

		public eState State
		{ get { return _State; } }

		public string Text
		{ get { return _Text; } }
	}

}
