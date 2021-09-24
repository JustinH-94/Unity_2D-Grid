using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPath : MonoBehaviour
{
    List<Node> path = new List<Node>(); //List of nodes that lead from the starting node to ending node
    Node startNode; //Starting place for Agents
    Node endNode; // The goal
    Material m; //Material m to gain access to the game object's material
    Vector3 goal; //Vector3 of next waypoint

    float speed; //speed of the agent
    float accuracy; //accuracy of agent to waypoint's location
    float rotSpeed; //speed of which the agent will rotate
    public float turnTimer = 0;
    int currentNode; //the value of Node which the agent currently resides
    int spacing = 3; //distance between waypoints
    int goalX, goalY;
    public int movePoint = 6;
    bool collided; //bool to see if agents collided with each other
    bool moveEnd; //For when the cube makes its move
    bool turnEnd; //For When the cube uses all of its moves for the turn

    Node[,] grid;
    public GameObject prefabWP;
    public Material goalMat;
    public Material wallMat;

    public int startX, staryY; //int values to manually input each Agent's location

    private void Start()
    {
        m = GetComponent<Renderer>().material; //setting m to gain access to Material components
        speed = Random.Range(2.0f, 5.0f); //randomize speed
        accuracy = Random.Range(0.1f, 0.5f); //randomize accuracy
        rotSpeed = Random.Range(4f, 7f); //randomize rotation speed
        goalX = Random.Range(0, 9);
        goalY = Random.Range(0, 11);
        collided = false; //starting collided to false;

        CreateGrid(); //Method that creates the nodes/waypoints of the grid layout

        startNode = grid[startX, staryY];//set Node of Agents
        endNode = grid[goalX, goalY]; //End Node of where the agents have to reach
        startNode.IsWalkable = true; //setting the starting node to be walkable
        endNode.WayPoint.GetComponent<Renderer>().material = goalMat; //changing the material color of the end goal

        this.transform.position = new Vector3(startNode.WayPoint.transform.position.x, //start Agent's position to startNode's position
                                              this.transform.position.y,
                                              startNode.WayPoint.transform.position.z);
    }

    void CreateGrid()//Manually creates Nodes
    {
        grid = new Node[,]
        {
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(false),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(false),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
            {
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
                new Node(),
            },
        };

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].WayPoint = Instantiate(prefabWP, new Vector3(i * spacing, this.transform.position.y, j * spacing), Quaternion.identity);//places the waypoint prefab on top of the node in grid[,]
                if (!grid[i, j].IsWalkable) //in case the grid is not walkable
                    grid[i, j].WayPoint.GetComponent<Renderer>().material = wallMat; //set the node's material to be wall mat
                else
                    grid[i, j].Neighbors = getAdjacentNode(grid, i, j); //set the neighboring nodes of current node

            }

        }
    }

    List<Node> BFS(Node start, Node end) // Breadth First Search Algorithm
    {
        Queue<Node> toVisit = new Queue<Node>(); //declare Queue that will detect the next node(s) to visit
        List<Node> visited = new List<Node>(); //delcare list that will nodes that have been visited

        Node currentNode = start; //declare that it's current node is it's starting code
        currentNode.Depth = 0;

        toVisit.Enqueue(currentNode); //adds node to toVisit queue
        List<Node> finalPath = new List<Node>(); //declares list of the path to take to the destination
        while (toVisit.Count > 0) //while the toVisit queue is greater than 0 following code executes
        {
            currentNode = toVisit.Dequeue();//remove the last node

            if (visited.Contains(currentNode))//if current node is in the list of visited nodes, then move on
                continue;

            visited.Add(currentNode); //add current node not nodes that have been visited

            if (currentNode.Equals(end))
            {
                while (currentNode.Depth != 0)
                {
                    foreach (Node n in currentNode.Neighbors)
                    {
                        if (n.Depth == currentNode.Depth - 1)
                        {
                            finalPath.Add(currentNode); //adds all walkable nodes to finalpath
                            currentNode = n;
                            break;
                        }
                    }
                }
                finalPath.Reverse();//flips the list of finalPath for the agent's to trace back
                break;
            }
            else
            {
                foreach (Node n in currentNode.Neighbors)
                {
                    if (!visited.Contains(n) && n.IsWalkable)
                    {
                        n.Depth = currentNode.Depth + 1;
                        toVisit.Enqueue(n);
                    }
                }
            }
        }
        return finalPath;
    }

    List<Node> getAdjacentNode(Node[,] m, int i, int j)//detects if the nodes nearby the current node are walkable or not. if yes, then add it to temporary node list
    {
        List<Node> temp = new List<Node>();

        //Node Above
        if (i - 1 >= 0)
        {
            if (m[i - 1, j].IsWalkable)
                temp.Add(m[i - 1, j]);
        }
        //Node Below
        if (i + 1 < m.GetLength(0))
        {
            if (m[i + 1, j].IsWalkable)
                temp.Add(m[i + 1, j]);
        }
        //Node Left
        if (j - 1 >= 0)
        {
            if (m[i, j - 1].IsWalkable)
                temp.Add(m[i, j - 1]);
        }
        //Node Right
        if (j + 1 < m.GetLength(1))
        {
            if (m[i, j + 1].IsWalkable)
                temp.Add(m[i, j + 1]);
        }
        return temp;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))//once user presses enter, BFS() executes and each Agent will get their path set
        {
            this.transform.position = new Vector3(startNode.WayPoint.transform.position.x, this.transform.position.y, startNode.WayPoint.transform.position.z);
            currentNode = 0;

            path = BFS(startNode, endNode);
        }

        if (path.Count == 0) return;

        goal = new Vector3(path[currentNode].WayPoint.transform.position.x, this.transform.position.y, path[currentNode].WayPoint.transform.position.z); //set goal of next node
        Vector3 direct = goal - this.transform.position; //gets vector distance between the next node and current position
        if (!moveEnd && movePoint >= 1)
        {
            if (direct.magnitude > accuracy && !collided) //In case the agent's have collided and the magnitude of the vector direct is greater than accuracy...
            {
                //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direct), Time.deltaTime * rotSpeed);//rotate agent to next node
                //this.transform.Translate(0, 0, speed * Time.deltaTime);//and move to the next node
                this.transform.position = goal;
                moveEnd = true;
                movePoint--;
            }
            else
            {
                if (currentNode < path.Count - 1)
                    currentNode++;
            }
        }

        if (moveEnd)
        {
            turnTimer += Time.deltaTime;
            if(turnTimer >= 1)
            {
                turnTimer = 0;
                moveEnd = false;
            }

        }

        if(movePoint <= 0)
        {
            turnTimer += Time.deltaTime;
            if (turnTimer >= 5)
            {
                turnTimer = 0;
                movePoint = 6;
            }
        }

    }








    public class Node
    {
        GameObject wayPoint = new GameObject();
        List<Node> neighbors = new List<Node>();

        int depth;
        bool isWalkable;

        public int Depth { get => depth; set => depth = value; }
        public bool IsWalkable { get => isWalkable; set => isWalkable = value; }

        public GameObject WayPoint { get => wayPoint; set => wayPoint = value; }
        public List<Node> Neighbors { get => neighbors; set => neighbors = value; }

        public Node()
        {
            this.depth = -1;
            this.isWalkable = true;
        }

        public Node(bool isWalkable)
        {
            this.depth = -1;
            this.isWalkable = isWalkable;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            Node n = obj as Node;

            if ((System.Object)n == null)
                return false;
            if (this.wayPoint.transform.position.x == n.WayPoint.transform.position.x &&
                this.wayPoint.transform.position.z == n.WayPoint.transform.position.z)
                return true;
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Agent")
        {
            m.color = Color.magenta;//changes color of the agent if collision occurs
            this.transform.position = new Vector3(this.transform.position.x, 3, this.transform.position.z);//fix location of agent's x and x position while moving its y position
            collided = true;//set collision to true to lock out movement
        }

    }
}
