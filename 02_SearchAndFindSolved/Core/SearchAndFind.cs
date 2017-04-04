using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{

    public class SearchAndFind : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private SceneContainer CreateScene()
        {
            return new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = "March 2017",
                    CreatedBy = "mch",
                    Generator = "Handcoded with pride",
                    Version = 42,
                },
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Base",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent { Scale = float3.One },
                            new MaterialComponent
                            {
                                Diffuse = new MatChannelContainer { Color = ColorUint.Red.Tofloat3() },
                                Specular = new SpecularChannelContainer {Color = ColorUint.White.Tofloat3(), Intensity = 1.0f, Shininess = 4.0f}
                            },
                            SimpleMeshes.CreateCuboid(new float3(100, 20, 100))
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Arm01",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent {Translation=new float3(0, 60, 0),  Scale = float3.One },
                                    new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer { Color = ColorUint.Green.Tofloat3() },
                                        Specular = new SpecularChannelContainer {Color = ColorUint.White.Tofloat3(), Intensity = 1.0f, Shininess = 4.0f}
                                    },
                                    SimpleMeshes.CreateCuboid(new float3(20, 100, 20))
                                },
                                

                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Arm02Rot",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new TransformComponent {Translation=new float3(-20, 40, 0),  Rotation = new float3(0.35f, 0, 0), Scale = float3.One},
                                        },
                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Arm02",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new TransformComponent {Translation=new float3(0, 40, 0),  Scale = float3.One },
                                                    new MaterialComponent
                                                    {
                                                        Diffuse = new MatChannelContainer { Color = ColorUint.Yellow.Tofloat3() },
                                                        Specular = new SpecularChannelContainer {Color = ColorUint.White.Tofloat3(), Intensity = 1.0f, Shininess = 4.0f}
                                                    },
                                                    SimpleMeshes.CreateCuboid(new float3(20, 100, 20))
                                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Arm03Rot",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new TransformComponent {Translation=new float3(20, 40, 0),  Rotation = new float3(0.25f, 0, 0), Scale = float3.One},
                                        },
                                        Children = new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Arm03",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new TransformComponent {Translation=new float3(0, 40, 0),  Scale = float3.One },
                                                    new MaterialComponent
                                                    {
                                                        Diffuse = new MatChannelContainer { Color = ColorUint.Blue.Tofloat3() },
                                                        Specular = new SpecularChannelContainer {Color = ColorUint.White.Tofloat3(), Intensity = 1.0f, Shininess = 4.0f}
                                                    },
                                                    SimpleMeshes.CreateCuboid(new float3(20, 100, 20))
                                                }
                                            },
                                        }
                                    }
                                }
                                            },
                                        }
                                    }
                                }
                            },
                        }
                    },
                }
            };
        }

        private TransformComponent _upperArm;
        private TransformComponent _foreArm;
        private float _upperArmVel;
        private float _foreaArmVel;
        private float _upperAngle;
        private float _foreaAngle;

        SceneNodeContainer FindNodeByName(IEnumerable<SceneNodeContainer> listToSearchIn, string NameToFind)
        {
            foreach (var child in listToSearchIn)
            {
                if (child.Name == NameToFind)
                    return child;
                else
                {
                    if (child.Children != null)
                    {
                        var found = FindNodeByName(child.Children, NameToFind);
                        if (found != null)
                            return found;
                    }
                }
            }
            return null;
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            _scene = CreateScene();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);

            // _upperArm = _scene.Children.FindNodes(node => node.Name == "Arm02Rot").First()?.GetTransform();
            // _foreArm = _scene.Children.FindNodes(node => node.Name == "Arm03Rot").First()?.GetTransform();
            _upperArm = FindNodeByName(_scene.Children, "Arm02Rot")?.GetTransform();
            _foreArm = FindNodeByName(_scene.Children, "Arm03Rot")?.GetTransform();
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                // _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                // _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
                _upperArmVel = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _foreaArmVel = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                    _upperArmVel *= curDamp;
                    _foreaArmVel *= curDamp;
                }
            }


            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;



            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 20, -600, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            // Arms
            _upperAngle += _upperArmVel;
            _foreaAngle += _foreaArmVel;
            _upperArm.Rotation = new float3(_upperAngle, 0, 0);
            _foreArm.Rotation = new float3(_foreaAngle, 0, 0);



            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

    }
}