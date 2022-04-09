using AStarPathFinding;
using AStarPathFinding.UnityIntegration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    private AStarPathFinder pathFinder;

    [SerializeField] bool enableGizmos;

    [SerializeField] private Transform target;
    [SerializeField] private Transform ground;

    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LayerMask ignorePathLayers;
    [Range(0,100)]
    [SerializeField] private float moveSpeed;

    private Vector3 startPos;
    private Vector3 movePos;
    private float timer = 0f;

    private void Start()
    {
        //Plane scale must be multiplied by 10
        PathGrid pathGrid = new PathGrid(ground.position.x, ground.position.y, ground.position.z, ground.localScale.x * 10, ground.localScale.z * 10, 0.5f);
        pathFinder = new AStarPathFinder(pathGrid, this.transform, target, obstacleLayers, ignorePathLayers, true);
        var nodes = pathFinder.pathFinder.pathGrid.nodes;
        List<Node> nodeList = new List<Node>(nodes.GetLength(0) * nodes.GetLength(1));
        for (int i = 0; i < nodes.GetLength(1); i++)
        {
            for (int j = 0; j < nodes.GetLength(0); j++)
            {
                nodeList.Add(nodes[i, j]);
            }
        }
        startPos = this.transform.position;
        movePos = pathFinder.GetNextPos(startPos);
    }

    void Update()
    {
        if(timer >= 1)
        {
            startPos = movePos;
            movePos = pathFinder.GetNextPos(movePos);
            timer = 0;
        }
        else
        {
            timer += moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, movePos, timer);
        }
    }

    private void OnDrawGizmos()
    {
        if (!UnityEditor.Selection.gameObjects.Contains(transform.gameObject)) { return; }
        if (pathFinder == null || 
            pathFinder.pathFinder.finalPath == null ||
            !enableGizmos) return;

        Node[,] nodes = pathFinder.pathFinder.pathGrid.nodes;
        var finalPath = pathFinder.pathFinder.finalPath;
        for (int i = 0; i < nodes.GetLength(1); i++)
        {
            for (int j = 0; j < nodes.GetLength(0); j++)
            {
                Node node = nodes[i, j];
                if (node.isObstacle)
                {
                    Gizmos.color = Color.red;
                }
                else if (node.dontIncludeToPath)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (node.ignoreable)
                {
                    Gizmos.color = Color.white;
                }
                else if (finalPath.Contains(node))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawCube(node.worldPosition.ToVector3(), Vector3.one);
            }

        }
    }

}
