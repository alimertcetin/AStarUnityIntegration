using System.Collections.Generic;
using UnityEngine;

namespace AStarPathFinding.UnityIntegration
{
    public class AStarPathFinder
    {
        public PathFinder pathFinder;

        public Transform pathFollower
        {
            get => startPoint;
            set
            {
                startPoint = value;
                UpdatePath();
            }
        }
        public Transform target
        {
            get => endPoint;
            set
            {
                endPoint = value;
                UpdatePath();
            }
        }
        private Transform startPoint;
        private Transform endPoint;

        public LayerMask ObstacleLayers
        {
            get => obstacleLayers;
            set
            {
                obstacleLayers = value;
                CreateObstacleNodes();
            }
        }
        public LayerMask IgnoreLayers
        {
            get => ignoreLayers;
            set
            {
                ignoreLayers = value;
                CreateObstacleNodes();
            }
        }
        LayerMask obstacleLayers;
        LayerMask ignoreLayers;

        public List<Node> obstacleNodes = new List<Node>();
        public List<Node> ignoredLayerNodes = new List<Node>();

        public bool MakeNeighborNodeObstacle
        {
            get => makeNeighborNodeObstacle;
            set
            {
                makeNeighborNodeObstacle = value;
                CreateObstacleNodes();
            }
        }
        private bool makeNeighborNodeObstacle;

        public AStarPathFinder(PathGrid pathGrid,
            Transform pathFollower, Transform target,
            LayerMask obstacleLayers, LayerMask ignoreLayers,
            bool makeNeighborNodeObstacle)
        {
            this.pathFinder = new PathFinder(pathGrid);
            this.startPoint = pathFollower;
            this.endPoint = target;
            this.obstacleLayers = obstacleLayers;
            this.ignoreLayers = ignoreLayers;
            this.makeNeighborNodeObstacle = makeNeighborNodeObstacle;

            CreateObstacleNodes();
            UpdatePath();
        }

        public void UpdatePath()
        {
            pathFinder.FindPath(startPoint.position.ToVec3(), endPoint.position.ToVec3());
        }

        public void CreateObstacleNodes()
        {
            obstacleNodes.Clear();
            for (int z = 0; z < pathFinder.pathGrid.GridSizeZ; z++)
            {
                for (int x = 0; x < pathFinder.pathGrid.GridSizeX; x++)
                {
                    var node = pathFinder.pathGrid.nodes[x, z];
                    node.ignoreable = Physics.CheckSphere(node.worldPosition.ToVector3(), pathFinder.pathGrid.NodeRadius, ignoreLayers);
                    node.isObstacle = Physics.CheckSphere(node.worldPosition.ToVector3(), pathFinder.pathGrid.NodeRadius, obstacleLayers);

                    if (node.isObstacle) obstacleNodes.Add(node);
                    if (node.ignoreable) ignoredLayerNodes.Add(node);
                }
            }

            if (makeNeighborNodeObstacle)
            {
                MakeNeighborObstacle();
            }
        }

        public void MakeNeighborObstacle()
        {
            var newObstacleNodes = new List<Node>();
            for (int i = 0; i < obstacleNodes.Count; i++)
            {
                List<Node> neighbors = pathFinder.pathGrid.GetNeighborNodes(obstacleNodes[i]);
                for (int j = 0; j < neighbors.Count; j++)
                {
                    if (neighbors[j].isObstacle) continue;

                    neighbors[j].isObstacle = true;
                    newObstacleNodes.Add(neighbors[j]);
                }
            }
            obstacleNodes.AddRange(newObstacleNodes);
        }


        public void UpdateNode(Vector3 previousPos, Vector3 nextPos)
        {
            pathFinder.pathGrid.UpdateNode(previousPos.ToVec3(), nextPos.ToVec3());
        }

        public void UpdateNode(Node node, Vector3 nextPos)
        {
            pathFinder.pathGrid.UpdateNode(node, nextPos.ToVec3());
        }

        public Node NodeFromWorldPosition(Vector3 currentPos)
        {
            return pathFinder.pathGrid.NodeFromWorldPosition(currentPos.ToVec3());
        }

        public List<Node> GetNeighborNodes(Node current)
        {
            return pathFinder.pathGrid.GetNeighborNodes(current);
        }

        public Node GetNeighbor(Node current, Vector3 direction)
        {
            return pathFinder.pathGrid.GetNeighbor(current, direction.ToVec3());
        }


        public Vector3 GetClosestPosition(Vector3 currentPosition)
        {
            return pathFinder.GetClosestPosition(currentPosition.ToVec3()).ToVector3();
        }

        public Vector3 GetNextPos(Vector3 currentPosition)
        {
            return pathFinder.GetNextPos(currentPosition.ToVec3()).ToVector3();
        }

        public Node GetClosest(List<Node> nodes, Vector3 currentPosition)
        {
            return pathFinder.pathGrid.GetClosest(nodes, currentPosition.ToVec3());
        }

        public Node GetClosestNext(List<Node> nodes, Vector3 currentPosition)
        {
            return pathFinder.pathGrid.GetClosestNext(nodes, currentPosition.ToVec3());
        }
    }

}
