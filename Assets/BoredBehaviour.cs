using System.Collections.Generic;
using UnityEngine;


//https://www.youtube.com/watch?v=OCd7terfNxk
public class BoredBehaviour : StateMachineBehaviour
{
    [SerializeField] Vector2 minxMaxBoredTimer = new(6, 12);
    [SerializeField] float timeUntilBored;

    [field: SerializeField] private List<WeightedProductSelectionItem<int>> weightedRandomQuantitySelection;
    WeightedRandomBag<int> boredAnimationRandomWeights = new();
    bool initializedRandomWeights = false;

    bool isBored;
    private float idleTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!initializedRandomWeights)
        {
            boredAnimationRandomWeights.Init(weightedRandomQuantitySelection);
        }

        ResetIdle(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isBored == false)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                isBored = true;

                int boredAnimation = boredAnimationRandomWeights.GetRandom();
                animator.SetFloat("BoredAnimation", boredAnimation);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f)
        {
            ResetIdle(animator);
        }
    }

    private void ResetIdle(Animator animator)
    {
        isBored = false;
        idleTimer = 0;

        timeUntilBored = Random.Range(minxMaxBoredTimer.x, minxMaxBoredTimer.y);

        animator.SetFloat("BoredAnimation", 0);
    }
}
