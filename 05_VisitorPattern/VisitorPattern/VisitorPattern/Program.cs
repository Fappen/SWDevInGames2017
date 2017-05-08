using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;

namespace VisitorPattern
{
    public class Visitor
    {
        public virtual void Visit(Group g) {}
        public virtual void Visit(Cuboid c) { }
        public virtual void Visit(Sphere s) { }
    }


    public class GraphicsObject
    {
        public virtual void Accept(Visitor v)
        {
        }

        public string Name;
    }

    public class Group : GraphicsObject
    {
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
        public List<GraphicsObject> Children;
    }

    public class Sphere : GraphicsObject
    {
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
        public float Radius;
    }

    public class Cuboid : GraphicsObject
    {
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
        public float Length;
        public float Width;
        public float Depth;
    }

    public class Renderer : Visitor
    {
        public override void Visit(Group g)
        {
            Console.WriteLine("A Group!");
            foreach (var child in g.Children)
                child.Accept(this);
        }

        public override void Visit(Sphere s)
        {
            Console.WriteLine("Greets from the Sphere " + s.Radius);
        }

        public override void Visit(Cuboid c)
        {
            Console.WriteLine("I am a Cuboid " + c.Length + ", " + c.Width + ", " + c.Depth);
        }
    }

    class JSONXporter : Visitor
    {
        public override void Visit(Group g)
        {
            // TODO
        }

        public override void Visit(Cuboid c)
        {
            // TODO
        }

        public override void Visit(Sphere s)
        {
            // TODO
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            /* Group g1 = new Group();
            g1.Children = new List<GraphicsObject>();

            Group g2 = new Group();
            g2.Children = new List<GraphicsObject>();

            Group g3 = new Group();
            g3.Children = new List<GraphicsObject>();

            Cuboid c1 = new Cuboid();
            c1.Length = 42;
            c1.Width = 4711;
            c1.Depth = 99;

            Cuboid c2 = new Cuboid();
            c2.Length = 22;
            c2.Width = 11;
            c2.Depth = 33;

            Sphere s1 = new Sphere();
            s1.Radius = 3.1415f;

            Sphere s2 = new Sphere();
            s2.Radius = 12;

            g3.Children.Add(c2);
            g2.Children.Add(c1);
            g2.Children.Add(s1);
            g2.Children.Add(g3);
            g1.Children.Add(g2);
            g1.Children.Add(s2);

            GraphicsObject root = g1;*/

            GraphicsObject root =
                new Group
                {
                    Children = new List<GraphicsObject>
                    {
                        new Group
                        {
                            Children = new List<GraphicsObject>
                            {
                                new Cuboid
                                {
                                    Length = 42,
                                    Width = 4711,
                                    Depth = 99
                                },
                                new Sphere
                                {
                                    Radius = 3.1415f,
                                },
                                new Group
                                {
                                    Children = new List<GraphicsObject>
                                    {
                                        new Cuboid
                                        {
                                            Length = 22,
                                            Width = 11,
                                            Depth = 33
                                        }
                                    }
                                }
                            }
                        },
                        new Sphere
                        {
                            Radius = 12,
                        }
                    }
                };


            root.Accept(new Renderer());

            Console.ReadKey();
        }
    }
}
