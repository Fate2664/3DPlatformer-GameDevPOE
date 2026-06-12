﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class BossWanderState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly GraphBase<Transform> graph;
        private GraphNode<Transform> currentNode;
        private GraphNode<Transform> previousNode;

        public BossWanderState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, GraphBase<Transform> graph) :
            base(enemyBase, animator)
        {
            this.graph = graph;
            this.agent = agent;
        }

        public override void OnEnter()
        {
            animator.CrossFade(walkHash, crossFadeDuration);

            currentNode ??= graph.Nodes[0];
            agent.SetDestination(currentNode.Value.position);
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                SelectNextNode();
            }
        }

        private void SelectNextNode()
        {
            if (currentNode.Neighbors.Count == 0) return;
            
            GraphNode<Transform> nextNode;
            if (currentNode.Neighbors.Count == 1)
            {
                nextNode = currentNode.Neighbors[0];
            }
            else
            {
                do
                {
                    int index = Random.Range(0, currentNode.Neighbors.Count);
                    nextNode = currentNode.Neighbors[index];
                }
                while (nextNode == previousNode);
            }
            
            previousNode = currentNode;
            currentNode = nextNode;
            
            agent.SetDestination(nextNode.Value.position);
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
        
    }
}