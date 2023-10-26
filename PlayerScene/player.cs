using Godot;
using System;

public partial class player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public Camera3D cam;
	public float MouseSensitivity = 0.5f;
	public override void _Ready()
	{
		base._Ready();
		cam = GetNode<Camera3D>("PlayerCam/Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
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
			float mouseAngleY = Mathf.DegToRad(mouseEvent.Relative.Y * MouseSensitivity);
			float mouseAngleX = Mathf.DegToRad(mouseEvent.Relative.X * MouseSensitivity * -1f);
			cam.RotateX(mouseAngleY);
			cam.RotateY(mouseAngleX);
			Vector3 cameraRotation = new Vector3();
			cameraRotation.X = (Mathf.Clamp(cam.Rotation.X, -7 * Mathf.Pi / 18f, 7 * Mathf.Pi / 18f));
			cameraRotation.Y = cam.Rotation.Y;
			cam.Rotation = cameraRotation;
			
		}

	}
}
