# The Division™ Echo FX in Unity3D
This project shows how to implement The Division™ ECHO recording fx in Unity3D.

> The Division is trademark of Ubisoft Entertainment in the U.S. and/or other countries

## Echo rendering

In the project you can find a demo scene at **Scenes/EchoFX** that will show a demo Echo recording as you move along the scene (check in the Editor view where the ECHO prefab is located).

The scene needs to use the ECHO post process effect in order to render the Echos. The current ECHO prefab contains a post process volume which sets the _Opacity parameter of the post process to 1. 

The EchoRenderer on the ECHO prefab requires an EchoData file which contains the position, normals and lightdirection in order to render the ECHO. These scenes are created with the EchoData creation tool.

## EchoData creation tool

There is a scene in the project that has been used for the creation of the demo EchoData. It is recommended to use the same scene for the creation of other scenes.

Open the scene **Scenes/EchoTool** and open the creation tool located at the menu **Tools/ECHO Creation**.

This tool needs an EchoData reference in order to allow you to start adding points to the scene. You can create a new EchoData file right clicking in the project view and select EchoData or simply duplicate the existing demo EchoData file.

Once a reference has been set on the tool you can load the content clicking Load or simply start casting points if the echo data is empty (or if you want to ovewrite the current content).

- You can tweak the density of points and radius of the brush.
- Clear the current editing points clicking the Clear button. This does not clear the content of the EchoData file.
- Save the current Echo scene in the EchoData file clicking the Save button. In order to clear the content of the file click Clear and Save.

A couple considerations:
- The tool will only cast points on one mesh at the time.
- In order to cast points on a mesh, it needs a MeshCollider component.

Click "Save" to store all the created points in the EchoData file.

Use this EchoData file on the EchoRenderer component of the ECHO prefab in order to render the created scene.

If the scene was created correctly, it should show up in the EditorView when the object with a EchoRenderer is selected.
