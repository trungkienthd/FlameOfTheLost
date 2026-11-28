Magic Circle Selectors includes six selector prefabs and come with the following functionality:

- Snap to parent: Parenting the prefab to the desired target will automatically move the selector to the pivot of the parent.

- Scale to parent: Parenting the prefab to the desired target will scale the selector to the bounds of the mesh renderer of the parent or the first child mesh renderer. If no parent is found, it will return to its initial scale.

- Animations: Each selector comes with a default animation and a "selected" animation. Toggle the "selected" state on and off by calling Select() in the Selector script.

See the Example scene to try out this functionality.


Selector script properties:

 - Color all Sprites: When enabled, all sprite renderers under the prefab will be assigned the color in the Selector Color property. Disable if you wish to color each sprite manually.
 	- Selector Color: The color to assign to all sprites in the prefab. Only works when Color all Sprites is enabled.

 - Scale Parent Override: If you want the selector to snap and scale to something other than the parent (e.g. the parent's pivot is not where you want the selector to go), assign a different Transform to this field and it will snap and scale to this instead. Otherwise, leave this field blank

 - Use property block: Enabled by default. Uses property block to assign values to the material to reduce draw calls.

 - Animation Properties:

	- Selected Rotation Speed: The rotation speed of the "Selected" state animation
	- Unselected Intensity: The intensity property for the default animation. Only applies to the flame intensity of Selector 5 and the electricity intensity of Selector 6
	- Selected Intensity: The intensity property for the "Selected" state animation. Only applies to the flame intensity of Selector 5 and the electricity intensity of Selector 6