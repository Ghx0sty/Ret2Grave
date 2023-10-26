using Godot;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

[Tool]
public partial class myblocks : StaticBody3D
{
	//Initialization of the variables; includes the Collision + Mesh objects that require to be modified. The exact node is fetched at _Ready(),
	//which is triggered once after the project is built, and every time the game is run.

	public MeshInstance3D MyMesh;
	public CollisionShape3D MyCol;
	public string basepath = "res://addons/myaddon/";

    //Enums + enum variables, to populate the exports. Add the name of your custom resources here;

    public BlockColors _BlockColor;
    public Blocktextures _BlockTexture;
    public BlockShapes _BlockShape;
    public enum BlockShapes
	{
		wall,
		floor,
		block,
		door,
		ramp
	}
	public enum Blocktextures
	{
		marble,
		wood
	}

	public enum BlockColors
	{
		green,
		blue,
		lightblue,
		orange,
		red,
		white,
		yellow,
		black,
	}

	//Exports for the dropdown menu to appear in the editor

	[ExportGroup("Block Properties")]
	[Export]
	public Blocktextures BlockTexture
	{
        get => _BlockTexture;
		set
		{
            _BlockTexture = value;
			updateTexture(makepath(0));
            updateUV();
        }
    }

	[Export]
	public BlockShapes BlockShape
	{
        get => _BlockShape;
        set
        {
            _BlockShape = value;
            updateShape(makepath(1), makepath(3)); 
			updateUV();
        }
    }

	[Export]
	public BlockColors BlockColor
	{
		get => _BlockColor;
		set
		{
			_BlockColor = value;
            updateColor(makepath(2)); 
        }
	}

	//Is used in all cases to create a full path for the resource requested. The currently set request in the enums is fetched in order to
	//create the path and choose the right element.
	public string makepath(int option)
    {
		string fullpath;
		string optionpath = "";
		string ending = "";

        switch (option)
        {
			case 0:
				ending = _BlockTexture.ToString() + ".tres";
				optionpath = "textures/";
				break;
			case 1:
				ending = _BlockShape.ToString() + ".res";
				optionpath = "blocks/";
				break;
			case 2:
				ending = _BlockColor.ToString() + ".txt";
				optionpath = "colors/";
				break;
			case 3:
				ending = _BlockShape.ToString() + "shape.tres";
				optionpath = "blocks/";
				break;
			default:
				GD.Print("You should *NEVER* see this");
				break;
        }
		fullpath = basepath + optionpath + ending;
		return fullpath;
    }

	//The UpdateColor method, which fetches the data in the .txt, sets the albedo color of the override material and calls for a UV update
	//because the duplicated texture's default UV is different.
	public void updateColor(string fullpath)
	{
		using var file = FileAccess.Open(fullpath, FileAccess.ModeFlags.Read);
		var newmat = new StandardMaterial3D();
		float r = file.GetLine().ToFloat();
		float g = file.GetLine().ToFloat();	
		float b = file.GetLine().ToFloat();
		newmat = (StandardMaterial3D)ResourceLoader.Load(makepath(0)).Duplicate();
		newmat.AlbedoColor = new Color(r, g, b);
		MyMesh.MaterialOverride = newmat;  //Goodjob you fixed it
		updateUV();
	}
	
	//Very simple. Just fetches the Mesh and collision, and sets it to the required variable. It takes two paths; the path to the .res (mesh) and .tres (shape)
	public void updateShape(string meshpath, string shapepath)
	{
		MyMesh.Mesh = (Mesh)ResourceLoader.Load(meshpath).Duplicate();
		MyCol.Shape = (Shape3D)ResourceLoader.Load(shapepath).Duplicate();
		updateUV();
    }

	//Even simpler. Fetches the texture specified, sets it, and then re-checks the color + UV to make sure the shape's resolution is fair.
	public void updateTexture(string fullpath)
	{
        MyMesh.MaterialOverride = (StandardMaterial3D)ResourceLoader.Load(fullpath).Duplicate(); ;
        updateColor(makepath(2));
    }

	//Updates the UV by fetching the size of the current mesh, then based off a basic calculation, sets the UV to something sensible.
	//This takes no overload parameters.
	public void updateUV()
	{
		Vector3 meshshape = MyMesh.GetAabb().Size;
		float scalefactor = (meshshape.X * meshshape.Y * meshshape.Z)/3;
		if (scalefactor < 3)
		{
			scalefactor = (meshshape.X * meshshape.Y * meshshape.Z) / 1.5f;
		}
		else if (scalefactor > 20)
		{
			scalefactor = (meshshape.X * meshshape.Y * meshshape.Z) / 6;

        }
		var material = (StandardMaterial3D)MyMesh.GetActiveMaterial(0).Duplicate();
		material.Uv1Scale = new Vector3(scalefactor, scalefactor, scalefactor);
        MyMesh.MaterialOverride = material;
    }

	// Called when the node enters the scene tree for the first time. Initializes the objects to edit on runtime
	public override void _Ready()
	{
		MyCol = GetNode<CollisionShape3D>("collision");
		MyMesh = GetNode<MeshInstance3D>("texture");
		updateTexture(makepath(0));
		updateShape(makepath(1), makepath(3));
		updateUV();
    }

}