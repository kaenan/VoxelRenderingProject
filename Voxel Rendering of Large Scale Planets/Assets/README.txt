Voxel Rendering of Large-Scale Planets Tool.

---------------------
HOW TO USE
---------------------
Currently there is a tool already created and setup, ready to be used.

The tool is named 'FinalVersion'. 
There is also a Terrain Colour file named 'TerrainColour1' and a material attached named 'PlanetMaterial1' which are already setup for the tool.
To use the tool as is, select the 'FinalVersion' and alter the following parameters as you like:

- Planet Size: Sets the default diameter of the planet.
- Noise Threshold: Determines the threshold of the iso values of the voxels to be used to created the mesh.
- Water: This is a prefab for the water layer. The prefab itself should not be changed, however, it can be removed if a water layer isn't required for the planet.
- Playable: Check this option if the planet is intended to be used as a playable environment.

- Terrain
	- Scale: Alters the roughness for the terrain.
	- Height Multiplier: Increases the height of the terrain.
	- Offset: Offsets the coordinates of where the noise values are returned.

- Caves
	- Cave Scale: The density of the cave pathways.
	- Size Multiplier: Increases the width of the pathways.
	- Cave Offset: Offsets the coordinates of where the noise values are returned.

- Trees
	- Tree Density: The chance a vertex has to instansiate an asset. (Higher the number, the lower the chance e.g. 200 is a 1 in 200 chance of instansiating an asset).
	- Tree Prefab: An array of all the assets that could be instansiated.

------------------------------------------------
GENERATING MULTIPLE PLANETS USING THE SAME TOOL
------------------------------------------------
When generating multiple planets using the same tool, a new Terrain Colour and Material MUST be created, otherwise the material of the previously generated planet will be overwritten.

To setup a new Terrain Colour right click the explore window and select the option 'Terrain Colour'. When created there will already be a material assigned to the material field but this will have to be changed.
Then right click again and create a new Material. Changed the material shader type to: Shader Graphs/Terrain.
Then assign this material to the Terrain Colour you just created.
Then assign the Terrain Colour to the Terrain Colour field in the tool.

--------------------
CREATING A NEW TOOL
--------------------
When creating a new tool, you will need to assign a new Terrain Colour to the Terrain Colour field, AND assign the Compute Shader files to their respective field, OTHERWISE THE TOOL WILL NOT WORK.

The Compute Shader fields and their respective :

- Generate Chunks: Compute Shader file = MarchWithTexture
- Noise Texture: Compute Shader file = RenderTextureCompute
- Terraform Compute: Compute Shader file = TerraformCompute

