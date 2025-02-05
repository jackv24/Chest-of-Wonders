﻿using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Linq;


namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Script Control/Standalone Only")]
	[Description("Check a property on a script and return if it's equal or not to the check value")]
	public class CheckProperty : ConditionTask, ISubParametersContainer {

		[SerializeField] /*[IncludeParseVariables]*/
		protected ReflectedFunctionWrapper functionWrapper;
		[SerializeField]
		protected BBParameter checkValue;
		[SerializeField]
		protected CompareMethod comparison;

		BBParameter[] ISubParametersContainer.GetSubParameters(){
			return functionWrapper != null? functionWrapper.GetVariables() : null;
		}

		private MethodInfo targetMethod{
			get {return functionWrapper != null? functionWrapper.GetMethod() : null;}
		}

		public override System.Type agentType{
			get
			{
				if (targetMethod == null){ return typeof(Transform); }
				return targetMethod.IsStatic? null : targetMethod.RTReflectedType();
			}
		}

		protected override string info{
			get
			{
				if (functionWrapper == null){
					return "No Property Selected";
				}
				if (targetMethod == null){
					return string.Format("<color=#ff6457>* {0} *</color>", functionWrapper.GetMethodString() );
				}
				var mInfo = targetMethod.IsStatic? targetMethod.RTReflectedType().FriendlyName() : agentInfo;
				return string.Format("{0}.{1}{2}", mInfo, targetMethod.Name, OperationTools.GetCompareString(comparison) + checkValue.ToString());
			}
		}

		public override void OnValidate(ITaskSystem ownerSystem){
			if (functionWrapper != null && functionWrapper.HasChanged()){	
				SetMethod(functionWrapper.GetMethod());
			}
			if (functionWrapper != null && targetMethod == null){
				Error(string.Format("Missing Property '{0}'", functionWrapper.GetMethodString()));
			}
		}

		//store the method info on agent set for performance
		protected override string OnInit(){

			if (targetMethod == null)
				return "CheckProperty Error";

			try
			{
				functionWrapper.Init(targetMethod.IsStatic? null : agent);
				return null;
			}
			catch {return "CheckProperty Error";}
		}

		//do it by invoking method
		protected override bool OnCheck(){

			if (functionWrapper == null){
				return true;
			}

			if (checkValue.varType == typeof(float)){
				return OperationTools.Compare( (float)functionWrapper.Call(), (float)checkValue.value, comparison, 0.05f );
			}
			if (checkValue.varType == typeof(int)){
				return OperationTools.Compare( (int)functionWrapper.Call(), (int)checkValue.value, comparison);
			}
			return object.Equals( functionWrapper.Call(), checkValue.value );			
		}

		void SetMethod(MethodInfo method){
			if (method != null){
				functionWrapper = ReflectedFunctionWrapper.Create(method, blackboard);
				checkValue = BBParameter.CreateInstance( method.ReturnType, blackboard);
				comparison = CompareMethod.EqualTo;			
			}
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR

		protected override void OnTaskInspectorGUI(){

			if (!Application.isPlaying && GUILayout.Button("Select Property")){

				var menu = new UnityEditor.GenericMenu();
				if (agent != null){
					foreach(var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
						menu = EditorUtils.GetInstanceMethodSelectionMenu(comp.GetType(), typeof(object), typeof(object), SetMethod, 0, true, true, menu);
					}
					menu.AddSeparator("/");
				}
				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(object))){
					menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 0, true, true, menu);
					if (typeof(UnityEngine.Object).IsAssignableFrom(t)){
						menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 0, true, true, menu);
					}
				}
				if ( NodeCanvas.Editor.NCPrefs.useBrowser){ menu.ShowAsBrowser("Select Property", this.GetType()); }
				else { menu.ShowAsContext(); }
				Event.current.Use();
			}			

			if (targetMethod != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedType().FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
				GUILayout.EndVertical();

				GUI.enabled = checkValue.varType == typeof(float) || checkValue.varType == typeof(int);
				comparison = (CompareMethod)UnityEditor.EditorGUILayout.EnumPopup("Comparison", comparison);
				GUI.enabled = true;
				NodeCanvas.Editor.BBParameterEditor.ParameterField("Value", checkValue);
			}
		}

		#endif
	}
}