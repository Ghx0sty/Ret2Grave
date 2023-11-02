using Godot;
using System;

public partial class player : CharacterBody3D
{
	public float Speed = 5.0f;
	public float JumpVelocity = 6.5f;
	public float sprintconst = 2.1f;
	public float Acceleration = 1f;
	public float AirAcceleration = 1f;
	public float Momentum = 1f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public Camera3D cam;
	public Node3D head;
	public float MouseSensitivity = 0.5f;
	public override void _Ready()
	{
		base._Ready();
		cam = GetNode<Camera3D>("PlayerCam/Camera3D");
		head = GetNode<Node3D>("PlayerCam");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		Transform3D camera = cam.GlobalTransform;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			AirAcceleration += 1.6f;
			Momentum += AirAcceleration / 1.8f;
		}

		if (Input.IsActionJustPressed("sprint"))
		{
			Momentum = sprintconst / 1.6f;
		}


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (camera.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			if (Momentum > 1f)
			{
				Momentum -= 0.1f;
			}

			if (IsOnFloor())
			{
					
				if (Input.IsActionPressed("sprint"))
				{
					velocity.X = direction.X * Speed * ((sprintconst + Momentum)/2);
					velocity.Z = direction.Z * Speed * ((sprintconst + Momentum/2));
				}
				else
				{
					velocity.X = direction.X * Speed * Momentum;
					velocity.Z = direction.Z * Speed * Momentum;
				}
			}
			else
			{
																							//Find a way to mix both sprint and air momentum here.
				velocity.X = direction.X * Speed * ((AirAcceleration + Momentum)/2);
				velocity.Z = direction.Z * Speed * ((AirAcceleration + Momentum)/2);
				if (AirAcceleration < 1)
				{
					AirAcceleration -= 0.1f;
				}
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.GetType() == typeof(InputEventMouseMotion))
		{
			var mouseEvent = (InputEventMouseMotion)@event;
			float mouseAngleY = Mathf.DegToRad(mouseEvent.Relative.Y * MouseSensitivity * -1f);
			float mouseAngleX = Mathf.DegToRad(mouseEvent.Relative.X * MouseSensitivity * -1f);
			cam.RotateX(mouseAngleY);
			head.RotateY(mouseAngleX);
			Vector3 cameraRotation = cam.Rotation;
			cameraRotation.X = (Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f)));
			cam.Rotation = cameraRotation;
			
		}

	}
}
