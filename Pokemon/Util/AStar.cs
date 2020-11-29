using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public static class AStar {

        public delegate void EdgeTrimmingFn(RbyTile goalTile, int edgeSet);

        public static void GenerateEdges(RbyMap map, int edgeSet, int destX, int destY, Action actions, int numFramesPerStep, EdgeTrimmingFn trimming, bool connectToNextEdgeset = false) {
            GenerateEdges(map, edgeSet, map.Tiles[destX, destY], actions, numFramesPerStep, trimming, connectToNextEdgeset);
        }

        public static void GenerateEdges(RbyMap map, int edgeSet, RbyTile goalTile, Action actions, int numFramesPerStep, EdgeTrimmingFn trimming, bool connectToNextEdgeset = false) {
            Dictionary<RbyTile, List<Action>> paths = new Dictionary<RbyTile, List<Action>>();

            foreach(RbyTile tile in map.Tiles) {
                List<Action> path = FindPath(tile, goalTile, numFramesPerStep);
                if(path != null) paths[tile] = path;
            }

            foreach(RbyTile tile in paths.Keys) {
                if((actions & Action.Up) != 0) AddDirectionalEdge(Action.Up, tile.CanMoveUp, tile, tile.Up, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Down) != 0) AddDirectionalEdge(Action.Down, tile.CanMoveDown, tile, tile.Down, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Left) != 0) AddDirectionalEdge(Action.Left, tile.CanMoveLeft, tile, tile.Left, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Right) != 0) AddDirectionalEdge(Action.Right, tile.CanMoveRight, tile, tile.Right, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.A) != 0) tile.AddEdge(edgeSet, new RbyEdge(Action.A, tile, edgeSet, 2));
                if((actions & Action.StartB) != 0) tile.AddEdge(edgeSet, new RbyEdge(Action.StartB, tile, edgeSet, 52));
                if((actions & Action.Delay) != 0) tile.AddEdge(edgeSet, new RbyEdge(Action.Delay, tile, edgeSet, 2));
            }

            trimming(goalTile, edgeSet);
        }

        public static void GenerateEdges(GscMap map, int edgeSet, GscTile goalTile, Action actions, int numFramesPerStep, bool connectToNextEdgeset = false) {
            Dictionary<GscTile, List<Action>> paths = new Dictionary<GscTile, List<Action>>();

            foreach(GscTile tile in map.Tiles) {
                List<Action> path = FindPath(tile, goalTile, numFramesPerStep);
                if(path != null) paths[tile] = path;
            }

            foreach(GscTile tile in paths.Keys) {
                if((actions & Action.Left) != 0) AddDirectionalEdge(Action.Left, tile.CanMoveLeft, tile, tile.Left, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.LeftA) != 0) AddDirectionalEdge(Action.LeftA, tile.CanMoveLeft, tile, tile.Left, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Right) != 0) AddDirectionalEdge(Action.Right, tile.CanMoveRight, tile, tile.Right, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.RightA) != 0) AddDirectionalEdge(Action.RightA, tile.CanMoveRight, tile, tile.Right, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Up) != 0) AddDirectionalEdge(Action.Up, tile.CanMoveUp, tile, tile.Up, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.UpA) != 0) AddDirectionalEdge(Action.UpA, tile.CanMoveUp, tile, tile.Up, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.Down) != 0) AddDirectionalEdge(Action.Down, tile.CanMoveDown, tile, tile.Down, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.DownA) != 0) AddDirectionalEdge(Action.DownA, tile.CanMoveDown, tile, tile.Down, goalTile, edgeSet, connectToNextEdgeset, numFramesPerStep, paths);
                if((actions & Action.StartB) != 0) tile.AddEdge(edgeSet, new GscEdge(Action.StartB, tile, edgeSet, 16));
                if((actions & Action.Select) != 0) tile.AddEdge(edgeSet, new GscEdge(Action.Select, tile, edgeSet, 52));
            }
        }

        private static void AddDirectionalEdge(Action action, bool canMove, RbyTile source, RbyTile dest, RbyTile goalTile, int edgeSet, bool connectToNextEdgeset, int numFramesPerStep, Dictionary<RbyTile, List<Action>> paths) {
            if(canMove && dest != null && paths.ContainsKey(dest) && !dest.InVisionOfTrainer && !dest.IsUnallowedWarp && dest.Map.Sprites[dest.X, dest.Y] == null) {
                int cost = Math.Abs(numFramesPerStep * (paths[dest].Count - paths[source].Count + 1));
                source.AddEdge(edgeSet, new RbyEdge(action, dest, dest == goalTile && connectToNextEdgeset ? edgeSet + 1 : edgeSet, cost));
            }
        }

        private static void AddDirectionalEdge(Action action, bool canMove, GscTile source, GscTile dest, GscTile goalTile, int edgeSet, bool connectToNextEdgeset, int numFramesPerStep, Dictionary<GscTile, List<Action>> paths) {
            if(canMove && dest != null && paths.ContainsKey(dest) && dest.Map.Warps[dest.X, dest.Y] == null) {
                int cost = Math.Abs(numFramesPerStep * (paths[dest].Count - paths[source].Count + 1));
                source.AddEdge(edgeSet, new GscEdge(action, dest, dest == goalTile && connectToNextEdgeset ? edgeSet + 1 : edgeSet, cost));
            }
        }

        public static void None(RbyTile goalTile, int edgeSet) {
        }

        public static void PickupItemFacingRight(RbyTile goalTile, int edgeSet) {
            goalTile.Left.RemoveEdge(edgeSet, Action.A);
            goalTile.Down.RemoveEdge(edgeSet, Action.Up);
            goalTile.Up.RemoveEdge(edgeSet, Action.Down);
        }

        public static void PickupItemFacingLeft(RbyTile goalTile, int edgeSet) {
            goalTile.Right.RemoveEdge(edgeSet, Action.A);
            goalTile.Down.RemoveEdge(edgeSet, Action.Up);
            goalTile.Up.RemoveEdge(edgeSet, Action.Down);
        }

        public static void PickupItemFacingUp(RbyTile goalTile, int edgeSet) {
            goalTile.Down.RemoveEdge(edgeSet, Action.A);
            goalTile.Left.RemoveEdge(edgeSet, Action.Right);
            goalTile.Right.RemoveEdge(edgeSet, Action.Left);
        }

        public static void PickupItemFacingDown(RbyTile goalTile, int edgeSet) {
            goalTile.Up.RemoveEdge(edgeSet, Action.A);
            goalTile.Left.RemoveEdge(edgeSet, Action.Right);
            goalTile.Right.RemoveEdge(edgeSet, Action.Left);
        }

        public static void Connect(RbyMap map, Direction directions, int edgeSet, int nextEdgeSet) {
            for(int x = 0; x < map.Width * 2; x++) {
                for(int y = 0; y < map.Height * 2; y++) {
                    if((directions & Direction.Up) != 0 && map.NorthConnection != null && y == 0) {
                        Connect(map, map.NorthConnection, x, y, edgeSet, nextEdgeSet);
                    }
                    if((directions & Direction.Down) != 0 && map.SouthConnection != null && x == map.Height * 2 - 1) {
                        Connect(map, map.NorthConnection, x, y, edgeSet, nextEdgeSet);
                    }
                    if((directions & Direction.Left) != 0 && map.WestConnection != null && x == 0) {
                        Connect(map, map.WestConnection, x, y, edgeSet, nextEdgeSet);
                    }
                    if((directions & Direction.Right) != 0 && map.EastConnection != null && x == map.Width * 2 - 1) {
                        Connect(map, map.EastConnection, x, y, edgeSet, nextEdgeSet);
                    }
                }
            }
        }

        public static void Connect(RbyMap map, RbyConnection connection, int x, int y, int edgeSet, int nextEdgeSet) {
            RbyTile destTile = connection.DestinationMap.Tiles[(x + connection.XAlignment) % 256, (y + connection.YAlignment) % 256];
            if(destTile != null) {
                map.Tiles[x, y].AddEdge(edgeSet, new RbyEdge(Action.Up, destTile, nextEdgeSet, 0));
            }
        }

        public delegate bool IllegalTileCallback(int direction, byte xCur, byte yCur, byte xDest, byte yDest);

        public static List<Action> FindPath(RbyTile start, RbyTile end, int numFramesPerStep) {
            RbyMap map = start.Map;
            return FindPath(start.X, start.Y, end.X, end.Y, map.Width, map.Height, numFramesPerStep, (direction, xCur, yCur, xDest, yDest) => {
                RbyTile curTile = map.Tiles[xCur, yCur];
                RbyTile destTile = map.Tiles[xDest, yDest];

                if(direction == 0 && !curTile.CanMoveUp) return true;
                else if(direction == 1 && !curTile.CanMoveLeft) return true;
                else if(direction == 2 && !curTile.CanMoveRight) return true;
                else if(direction == 3 && !curTile.CanMoveDown) return true;

                if(map.Sprites[xDest, yDest] != null) return true;
                if(destTile.InVisionOfTrainer) return true;

                return false;
            });
        }


        public static List<Action> FindPath(GscTile start, GscTile end, int numFramesPerStep) {
            GscMap map = start.Map;
            return FindPath(start.X, start.Y, end.X, end.Y, map.Width, map.Height, numFramesPerStep, (direction, xCur, yCur, xDest, yDest) => {
                GscTile curTile = map.Tiles[xCur, yCur];
                GscTile destTile = map.Tiles[xDest, yDest];

                if(direction == 0 && !curTile.CanMoveUp) return true;
                else if(direction == 1 && !curTile.CanMoveLeft) return true;
                else if(direction == 2 && !curTile.CanMoveRight) return true;
                else if(direction == 3 && !curTile.CanMoveDown) return true;

                if(map.Warps[xDest, yDest] != null) return true;

                return false;
            });
        }

        public static List<Action> FindPath(byte xStart, byte yStart, byte xEnd, byte yEnd, byte width, byte height, int numFramesPerStep, IllegalTileCallback illegalTileCallback) {
            AStarNode currentNode = new AStarNode(xStart, yStart, null, 0.0f, Distance(xStart, yStart, xEnd, yEnd));
            List<AStarNode> openList = new List<AStarNode>() { currentNode };
            List<AStarNode> closedList = new List<AStarNode>();
            while(openList.Count > 0) {
                openList.Sort((lhs, rhs) => lhs.FCost.CompareTo(rhs.FCost));
                currentNode = openList[0];

                if(currentNode.X == xEnd && currentNode.Y == yEnd) {
                    List<Action> path = new List<Action>();
                    while(currentNode.Parent != null) {
                        if(currentNode.X - currentNode.Parent.X < 0) path.Add(Action.Left);
                        else if(currentNode.X - currentNode.Parent.X > 0) path.Add(Action.Right);
                        else if(currentNode.Y - currentNode.Parent.Y < 0) path.Add(Action.Up);
                        else if(currentNode.Y - currentNode.Parent.Y > 0) path.Add(Action.Down);
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();

                    return path;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);
                for(int i = 0; i < 4; i++) {
                    int x = currentNode.X;
                    int y = currentNode.Y;
                    int xa = ((i * 2 + 1) % 3) - 1;
                    int ya = ((i * 2 + 1) / 3) - 1;
                    byte xDest = (byte) (x + xa);
                    byte yDest = (byte) (y + ya);

                    if(xDest < 0 || yDest < 0 || xDest > width * 2 - 1 || yDest > height * 2 - 1) {
                        continue;
                    }

                    if(illegalTileCallback(i, currentNode.X, currentNode.Y, xDest, yDest)) continue;

                    float gCost = currentNode.GCost + 1;
                    float hCost = Distance(xDest, yDest, xEnd, yEnd);
                    AStarNode destNode = new AStarNode(xDest, yDest, currentNode, gCost, hCost);

                    if(closedList.ContainsPosition(xDest, yDest) && destNode.GCost >= currentNode.GCost) {
                        continue;
                    }

                    if(!openList.ContainsPosition(xDest, yDest) || destNode.GCost < currentNode.GCost) {
                        openList.Add(destNode);
                    }
                }
            }

            return null;
        }

        private static float Distance(byte x1, byte y1, byte x2, byte y2) {
            float dx = x1 - x2;
            float dy = y1 - y2;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        private static float ManhattanDistance(byte x1, byte y1, byte x2, byte y2) {
            return MathF.Abs(x1-x2) + MathF.Abs(y1-y2);
        }

        private static bool ContainsPosition(this List<AStarNode> list, int x, int y) {
            return list.Find(node => node.X == x && node.Y == y) != null;
        }
    }

    public class AStarNode {

        public byte X;
        public byte Y;
        public AStarNode Parent;

        public float GCost;
        public float HCost;

        public float FCost {
            get { return GCost + HCost; }
        }

        public AStarNode(byte x, byte y, AStarNode parent, float gCost, float hCost) {
            X = x;
            Y = y;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }

        public override string ToString() {
            return X + "," + Y;
        }
    }
}
