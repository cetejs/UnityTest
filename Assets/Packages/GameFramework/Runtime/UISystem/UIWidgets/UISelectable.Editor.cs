namespace GameFramework
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using UnityEditor.Animations;

    public partial class UISelectable
    {
        [ContextMenu("Auto Generate Animation")]
        private void EnsureAnimatorController()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }

            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            if (animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = GenerateAnimatorController();
            }

            if (animator.runtimeAnimatorController == null)
            {
                return;
            }

            AnimatorController controller = (AnimatorController) animator.runtimeAnimatorController;
            GenerateTriggerableTransition(normalTrigger, controller);
            GenerateTriggerableTransition(highlightedTrigger, controller);
            GenerateTriggerableTransition(pressedTrigger, controller);
            GenerateTriggerableTransition(disabledTrigger, controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private AnimatorController GenerateAnimatorController()
        {
            string message = $"Create a new animator for the game object '{name}':";
            string path = EditorUtility.SaveFilePanelInProject("New Animation Controller", name, "controller", message);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            return controller;
        }

        private void GenerateTriggerableTransition(string triggerName, AnimatorController controller)
        {
            if (string.IsNullOrEmpty(triggerName))
            {
                return;
            }

            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                if (parameter.name == triggerName)
                {
                    return;
                }
            }

            AnimationClip clip = AnimatorController.AllocateAnimatorClip(triggerName);
            AssetDatabase.AddObjectToAsset(clip, controller);
            AnimatorState state = controller.AddMotion(clip);
            controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
            transition.duration = 0;
            transition.canTransitionToSelf = false;
            transition.AddCondition(AnimatorConditionMode.If, 0, triggerName);
        }
    }
#endif
}
