using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
namespace WEBGL_EXPORTER.GLTF
{
    public class ObjectExtrasAnimationController
    {
        //statics
        public static int globalIndex = -1;
        public static List<ObjectExtrasAnimationController> allUniqueAnimationControllers;

        //vars
        public int index = -1;

        //public Animator animator;
        public AnimatorController animatorController;

        //public List<int> animationClipIndices;
        public List<int> connectedNodeIndices;
        public float timeScale;
        public List<ObjectProperty> animLayers;
        public List<ObjectProperty> parameters;


        //used to know copies of states and triggers
        public List<AnimatorControllerParameter> animatorParameters;
        //private List<AnimatorState> animatorStates;//cant go here

        public static void Reset()
        {
            globalIndex = 0;
            allUniqueAnimationControllers = new List<ObjectExtrasAnimationController>();
        }
        public static int GetAnimatorIndex(Animator _animator, int _mono_node)
        {
            if (_animator == null)
                return -1;

            AnimatorController animator_controller = null;
            if (_animator.runtimeAnimatorController != null)
            {
                animator_controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(_animator.runtimeAnimatorController));
            }
            if (animator_controller == null)
                return -1;

            if (allUniqueAnimationControllers == null)
            {
                allUniqueAnimationControllers = new List<ObjectExtrasAnimationController>();

                allUniqueAnimationControllers.Add(new ObjectExtrasAnimationController(animator_controller,_animator, _mono_node));

                return allUniqueAnimationControllers.Count - 1;
            }
            else
            {
                foreach (ObjectExtrasAnimationController oam in allUniqueAnimationControllers) 
                { 
                    if (oam.isSameAnimationMixer(animator_controller))
                    {

                        oam.connectedNodeIndices.Add(_mono_node);
                        return oam.index;
                    }
                }
                allUniqueAnimationControllers.Add(new ObjectExtrasAnimationController(animator_controller, _animator, _mono_node));
                return allUniqueAnimationControllers.Count - 1;
            }
        }

        private bool isSameAnimationMixer(AnimatorController animator_controller)
        {
            if (animator_controller == animatorController)
                return true;
            return false;
        }

        private List<ObjectProperty> getConditions(AnimatorCondition[] _conditions, List<ObjectProperty> existing_list = null)
        {
            if (existing_list == null)
                existing_list = new List<ObjectProperty>();
            foreach (AnimatorCondition condition in _conditions)
            {
                List<ObjectProperty> _single_condition = new List<ObjectProperty>();
                for (int j = 0; j < animatorParameters.Count; j++)
                {
                    if (animatorParameters[j].name == condition.parameter)
                    {
                        _single_condition.Add(new ObjectProperty("param", j));
                        _single_condition.Add(new ObjectProperty("cond", GetConditionModeString(condition.mode)));
                        switch (animatorParameters[j].type)
                        {
                            case AnimatorControllerParameterType.Bool:
                            case AnimatorControllerParameterType.Trigger:
                                _single_condition.Add(new ObjectProperty("value", true));   // must only be compared to true, condition.mode will decide wether its true or false
                                break;
                            case AnimatorControllerParameterType.Float:
                            case AnimatorControllerParameterType.Int:
                                _single_condition.Add(new ObjectProperty("value", condition.threshold));
                                break;
                        }
                    }
                }

                existing_list.Add(new ObjectProperty("", _single_condition));
            }
            return existing_list;
        }
        
        private ObjectProperty getTransition(int from, int to, AnimatorTransition transition = null)
        {
            List<ObjectProperty> _single_transition = new List<ObjectProperty>();
            _single_transition.Add(new ObjectProperty("from", from));
            // GET TARGET STATE
            _single_transition.Add(new ObjectProperty("to", to));

            if (transition != null) //in default mode is null
            {
                List<ObjectProperty> _conditions = getConditions(transition.conditions);
                if (_conditions.Count > 0)
                    _single_transition.Add(new ObjectProperty("conditions", _conditions, true));
            }
            return new ObjectProperty("", _single_transition);
        }

        private ObjectProperty getStateTransition(int from, int to, AnimatorStateTransition transition)
        {
            List<ObjectProperty> _single_transition = new List<ObjectProperty>();
            List<ObjectProperty> _params = new List<ObjectProperty>();
            _single_transition.Add(new ObjectProperty("from", from));
            // GET TARGET STATE
            _single_transition.Add(new ObjectProperty("to", to));

            List<ObjectProperty> _conditions = getConditions(transition.conditions);
            if (_conditions.Count > 0)
                _single_transition.Add(new ObjectProperty("conditions", _conditions, true));

            if (transition.hasExitTime)
                _params.Add(new ObjectProperty("exitTime", transition.exitTime));
            if (transition.offset != 0)
                _params.Add(new ObjectProperty("offset",transition.offset));

            if (transition.interruptionSource != TransitionInterruptionSource.None)
            {
                switch (transition.interruptionSource)
                {
                    case TransitionInterruptionSource.Source:
                        _params.Add(new ObjectProperty("fromBreaks",true));
                        if (!transition.orderedInterruption) _params.Add(new ObjectProperty("orderedBreak", false));
                        break;
                    case TransitionInterruptionSource.Destination:
                        _params.Add(new ObjectProperty("toBreaks", true));
                        // Ordered break doesnt work here
                        break;
                    case TransitionInterruptionSource.SourceThenDestination:
                        _params.Add(new ObjectProperty("fromBreaks", true));
                        _params.Add(new ObjectProperty("toBreaks", true));
                        if (!transition.orderedInterruption) _params.Add(new ObjectProperty("orderedBreak", false));
                        break;
                    case TransitionInterruptionSource.DestinationThenSource:
                        _params.Add(new ObjectProperty("fromBreaks", true));
                        _params.Add(new ObjectProperty("toBreaks", true));
                        _params.Add(new ObjectProperty("fromPriority",false));
                        if (!transition.orderedInterruption) _params.Add(new ObjectProperty("orderedBreak", false));
                        break;
                }
            }

            _params.Add(new ObjectProperty("duration", transition.duration));
            _params.Add(new ObjectProperty("fixedDuration", transition.hasFixedDuration));

            if (_params.Count > 0)
                _single_transition.Add(new ObjectProperty("params", _params));
            //interruption source?
            return new ObjectProperty("", _single_transition);
        }

        private ObjectProperty getDualTransition(int from, int to, AnimatorTransition transition, AnimatorStateTransition state_transition)
        {
            List<ObjectProperty> _single_transition = new List<ObjectProperty>();
            List<ObjectProperty> _params = new List<ObjectProperty>();
            _single_transition.Add(new ObjectProperty("from", from));
            // GET TARGET STATE
            _single_transition.Add(new ObjectProperty("to", to));

            List<ObjectProperty> _conditions = getConditions(transition.conditions);
            _conditions = getConditions(state_transition.conditions, _conditions);
            if (_conditions.Count > 0)
                _single_transition.Add(new ObjectProperty("conditions", _conditions, true));

            if (state_transition.hasExitTime)
                _params.Add(new ObjectProperty("exitTime", state_transition.exitTime));
            if (state_transition.offset != 0)
                _params.Add(new ObjectProperty("offset", state_transition.offset));

            _params.Add(new ObjectProperty("duration", state_transition.duration));
            _params.Add(new ObjectProperty("fixedDuration", state_transition.hasFixedDuration));

            if (_params.Count > 0)
                _single_transition.Add(new ObjectProperty("params", _params));

            return new ObjectProperty("", _single_transition);
        }

        private void GetAllAnimatorStates(ref List<AnimatorState> animatorStateList, AnimatorStateMachine stateMachine)
        {
            // Get all simple states
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                animatorStateList.Add(state.state);
            }

            // Get All state machines states
            foreach (ChildAnimatorStateMachine subStateMachine in stateMachine.stateMachines)
            {
                GetAllAnimatorStates(ref animatorStateList, subStateMachine.stateMachine);
            }
        }

        public ObjectExtrasAnimationController(AnimatorController animator_controller, Animator _animator, int _mono_node)
        {
            connectedNodeIndices = new List<int>();
            connectedNodeIndices.Add(_mono_node);
            animLayers = new List<ObjectProperty>();
            parameters = new List<ObjectProperty>();
            timeScale = _animator.speed;

            animatorController = animator_controller;

            animatorParameters = new List<AnimatorControllerParameter>();
            foreach (AnimatorControllerParameter param in animatorController.parameters)
            {
                List<ObjectProperty> _single_param = new List<ObjectProperty>();
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        parameters.Add(new ObjectProperty("", new ObjectProperty(param.name, param.defaultBool)));
                        break;
                    case AnimatorControllerParameterType.Float:
                        parameters.Add(new ObjectProperty("",new ObjectProperty(param.name, param.defaultFloat)));
                        break;
                    case AnimatorControllerParameterType.Int:
                        parameters.Add(new ObjectProperty("", new ObjectProperty(param.name, param.defaultInt)));
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        List<ObjectProperty> triggerProperty = new List<ObjectProperty>();
                        triggerProperty.Add(new ObjectProperty("value", param.defaultBool));
                        _single_param.Add(new ObjectProperty(param.name, triggerProperty));
                        parameters.Add(new ObjectProperty("", _single_param));
                        break;
                }
                
                animatorParameters.Add(param);
            }
           
            for (int l = 0; l < animatorController.layers.Length; l++)
            {
                AnimatorControllerLayer animLayer = animatorController.layers[l];

                List<ObjectProperty> _single_layer = new List<ObjectProperty>();
                List<ObjectProperty> _states = new List<ObjectProperty>();
                List<ObjectProperty> _transitions = new List<ObjectProperty>();
                
                List<AnimatorState> _animator_states = new List<AnimatorState>();

                // LAYER BASIC PROERTIES
                _single_layer.Add(new ObjectProperty("name", animLayer.name));
                _single_layer.Add(new ObjectProperty("blendMode", animLayer.blendingMode.ToString().ToLower()));
                if (l!= 0) _single_layer.Add(new ObjectProperty("weight", animLayer.defaultWeight));
                // SAVE SYNCED DATA
                if (animLayer.syncedLayerIndex != -1)
                {
                    Debug.Log(animLayer.name);
                    Debug.Log(animLayer.stateMachine.stateMachines.Length);
                    Debug.Log(animLayer.stateMachine.states.Length);
                    // GET THE "FULL" LIST OF ALL ANIMATOR STATES IN SYNCED LAYER
                    AnimatorControllerLayer syncLayer = animatorController.layers[animLayer.syncedLayerIndex];
                    GetAllAnimatorStates(ref _animator_states, syncLayer.stateMachine);
                    //GetAllAnimatorStates(ref _animator_states, animLayer.stateMachine);
                    //animLayer.

                    List<int> _syncedClips = new List<int>();
                    // SYNCED LAYERS WILL HAVE ONLY THE INDEX OF THE CLIPS
                    Debug.Log(_animator_states.Count);
                    foreach (AnimatorState st in _animator_states)
                    {
                        if (st.motion != null)
                        {
                            if (st.motion.GetType() == typeof(AnimationClip))
                                _syncedClips.Add(ObjectExtrasAnimationClip.GetAnimationIndex(animLayer.GetOverrideMotion(st) as AnimationClip));
                            else
                            {
                                // BLEND TREE SECTION
                            }
                        }
                        else
                        {
                            _syncedClips.Add(ObjectExtrasAnimationClip.GetAnimationIndex(animLayer.GetOverrideMotion(st) as AnimationClip));
                        }
                    }

                    _single_layer.Add(new ObjectProperty("sync", animLayer.syncedLayerIndex));
                    if (animLayer.syncedLayerAffectsTiming) _single_layer.Add(new ObjectProperty("timing", true));
                    _single_layer.Add(new ObjectProperty("syncedClips", _syncedClips));
                }
                // SAVE ALL DATA
                else
                {
                    // GET THE "FULL" LIST OF ALL ANIMATOR STATES IN THIS LAYER
                    GetAllAnimatorStates(ref _animator_states, animatorController.layers[l].stateMachine);

                    // DEFAULT STATE TRANSITION, IF ITS INSIDE A SUBSTATE MACHINE, IT WILL TAKE IT NO MATTER
                    int defaultStateIndex = _animator_states.IndexOf(animLayer.stateMachine.defaultState);
                    if (animLayer.stateMachine.defaultState != null)
                        _single_layer.Add(new ObjectProperty("initialState", defaultStateIndex));

                    AddStateMachine(animLayer.stateMachine, ref _animator_states, ref _states, ref _transitions);

                    // DEFAULT TRANSITIONS GETS WRITTEN AT THE END, BECAUSE IF NO OTHER VALIDATION SUCCEEDS, DEFAULT IS TAKEN
                    if (defaultStateIndex != -1)
                        _transitions.Add(getTransition(-1, defaultStateIndex, null));

                    // FINALLY ADD THE MODIFIED LISTS TO THE LAYER LIST
                    _single_layer.Add(new ObjectProperty("states", _states, true));
                    _single_layer.Add(new ObjectProperty("transitions", _transitions, true));
                }

                // AND THE LAYER TO THE LAYERS ARRAY
                animLayers.Add(new ObjectProperty("", _single_layer));

            }
            //objectProperties.Add(new ObjectProperty("timeScale", animator_controller.speed));

            index = globalIndex;
            globalIndex++;

        }
        // == FIRST STEP == GET THE STATE MACHINE
        private void AddStateMachine(AnimatorStateMachine state_machine, ref List<AnimatorState> animator_states, ref List<ObjectProperty> _states, ref List<ObjectProperty> _transitions)
        {
            foreach (ChildAnimatorState child_states in state_machine.states)
            {
                //.. get each state data
                AddStateData(child_states.state, ref animator_states, ref _states, ref _transitions);
            }
            foreach (ChildAnimatorStateMachine child_state_machines in state_machine.stateMachines)
            {
                //.. call this function again
                AddStateMachine(child_state_machines.stateMachine, ref animator_states, ref _states, ref _transitions);
            }


            // ONLY IN SUBMACHINES WE CHECK "ENTRY STATE"...
            foreach (AnimatorTransition transition in state_machine.entryTransitions)
            {
                int destinationState = animator_states.IndexOf(transition.destinationState);
                if (destinationState != -1)
                    _transitions.Add(getTransition(-1, destinationState, transition));
            }
            // ... AND "ANY STATE" TRANSITIONS
            foreach (AnimatorStateTransition transition in state_machine.anyStateTransitions)
            {
                // CHECK FOR AT LEAST 1 CONDITION, ELSE, SKIP OR ERRORS WILL OCCUR
                if (transition.conditions.Length != 0)
                {
                    int destinationState = animator_states.IndexOf(transition.destinationState);
                    _transitions.Add(getStateTransition(-2, destinationState, transition));
                }
            }

        }
        private int GetParamID(string param_name)
        {
            for (int i = 0; i < animatorParameters.Count; i++)
            {
                if (animatorParameters[i].name == param_name)
                {
                    return i;
                }
            }
            return -1;
        }
        // == SECOND STEP == SAVE THE STATES DATA
        private void AddStateData(AnimatorState _state, ref List<AnimatorState> animator_states, ref List<ObjectProperty> _states, ref List<ObjectProperty> _transitions)
        {
            List<ObjectProperty> _single_state = new List<ObjectProperty>();

            // BASIC STATE DATA SUCH AS NAME, CLIP AND SPEED
            _single_state.Add(new ObjectProperty("name", _state.name));
            if (_state.speedParameterActive)
                _single_state.Add(new ObjectProperty("multParam", GetParamID(_state.speedParameter)));

            if (_state.timeParameterActive)
                _single_state.Add(new ObjectProperty("motionParam", GetParamID(_state.timeParameter)));

            if (_state.cycleOffsetParameterActive)
                _single_state.Add(new ObjectProperty("offsetParam", GetParamID(_state.cycleOffsetParameter)));
            else
                if (_state.cycleOffset != 0f)
                    _single_state.Add(new ObjectProperty("offset", _state.cycleOffset));



            // ANIMATION CLIP / BLEND TREE PROPERTIES
            if (_state.motion != null)     // CAN BE AN EMPTY STATE
            {
                if (_state.motion.GetType() == typeof(AnimationClip))  //might be BlendTree
                {
                    _single_state.Add(new ObjectProperty("clip", ObjectExtrasAnimationClip.GetAnimationIndex(_state.motion as AnimationClip)));
                    if (_state.speed != 1) _single_state.Add(new ObjectProperty("speed", _state.speed));
                }
                else
                {
                    //blend tree type
                }
            }

            // STATE TRANSITIONS DATA, MIGHT HAVE MORE THAN 1
            int state_index = animator_states.IndexOf(_state);
            if (_state.transitions.Length > 0)
            {
                // ADD INVERSED TO KEEP PTRIORITIES
                for (int i = _state.transitions.Length - 1; i >= 0; i--)
                {
                    if (_state.transitions[i].conditions.Length > 0 || _state.transitions[i].hasExitTime)
                    AddTransition(state_index, _state.transitions[i], ref animator_states, ref _transitions);
                }
            }

            // SAVE TO THE LIST OF ALL STATES IN THE REFERENCED LIST
            _states.Add(new ObjectProperty("", _single_state));
        }

        // == THIRD STEP == SAVE TRANSITIONS DATA
        private void AddTransition(int from_id, AnimatorStateTransition transition, ref List<AnimatorState> animator_states, ref List<ObjectProperty> transitions_list)
        {
            if (transition.destinationState != null)
            {
                // ADD IT NORMALLY, TRANSITION ENDS HERE
                int destinationState = animator_states.IndexOf(transition.destinationState);
                transitions_list.Add(getStateTransition(from_id, destinationState, transition));
            }
            else if (transition.destinationStateMachine != null)
            {
                // GET THE MACHINE STATE, AND GET ALL TRANSITIONS FROM WITHIN
                AddTransitionsFromStateMachine(from_id, transition, transition.destinationStateMachine, ref animator_states, ref transitions_list);
            }

        }

        // == FOURTH STEP == VALIDATE TRANSITIONS FROM STATE MACHINES
        private void AddTransitionsFromStateMachine(int from_id, AnimatorStateTransition last_transition , AnimatorStateMachine state_machine, ref List<AnimatorState> animator_states, ref List<ObjectProperty> transitions_list)
        {
            
            int default_index = animator_states.IndexOf(state_machine.defaultState); 

            // CHECK "ENTRY TRANSITIONS", THEY WILL BE CONNECTED TO THE STATE ID PROVIDED FROM BEFORE
            foreach (AnimatorTransition transition in state_machine.entryTransitions)
            {
                int destinationState = animator_states.IndexOf(transition.destinationState);
                if (destinationState != default_index)
                {  // DEFAULT TRANSITION MUST BE ADDED THE END
                    transitions_list.Add(getDualTransition(from_id, destinationState,transition,last_transition));
                }
            }
            // IN "DEFAULT" CASE, THERE IS NO CONDITION, SO WE TAKE THE CONDITION FROM PREVIOUS LAYER
            transitions_list.Add(getStateTransition(from_id, default_index, last_transition));
        }

        private string GetConditionModeString(AnimatorConditionMode mode)
        {
            return mode.ToString().ToLower();
        }
        public static void AddGLTFDataToExtras()
        {
            if (allUniqueAnimationControllers.Count > 0)
            {
                if (ExportToGLTF.options.exportAnimations)
                {
                    List<ObjectProperty> _animationControllers = new List<ObjectProperty>();
                    foreach (ObjectExtrasAnimationController oac in allUniqueAnimationControllers)
                    {
                        List<ObjectProperty> _anim_controller = new List<ObjectProperty>();
                        _anim_controller.Add(new ObjectProperty("name", oac.animatorController.name));
                        _anim_controller.Add(new ObjectProperty("nodes", oac.connectedNodeIndices));
                        if (oac.timeScale != 1f) _anim_controller.Add(new ObjectProperty("timeScale",oac.timeScale));
                        if (oac.parameters.Count > 0) _anim_controller.Add(new ObjectProperty("parameters", oac.parameters, true));
                        _anim_controller.Add(new ObjectProperty("layers", oac.animLayers,true));

                        _animationControllers.Add(new ObjectProperty("", _anim_controller));

                    }
                    ObjectMasterExtras.Add(new ObjectProperty("animationControllers", _animationControllers,true));
                }
            }
        }

    }
}
