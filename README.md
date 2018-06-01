# SimpleGui for Veldrid

An amateur WIP user interface tech demo written for the [Veldrid](https://github.com/mellinoe/veldrid) library using SDL for input.

![Preview](https://github.com/drogoganor/SimpleGui/blob/master/images/Example.png)

Veldrid already does include a reliable user interface: [ImGui](https://github.com/ocornut/imgui). But if that is not your style and you want to roll your own ui for Veldrid, this project might be a good starting place. Otherwise I cannot recommend this library for any serious use in an application.

## About

SimpleGui currently supports only a limited set of simple controls:

* Control
* Label
* Checkbox
* Button
* TextBox (no selection)

Uses the [TextRender](https://github.com/drogoganor/TextRender) library which is a similar WIP of mine.

Gui configuration including color themes are configured in gui.json in the application directory.

There's no handling of control focus or overlapping controls yet. Rendering is not batched and is naive - low performance if you have a lot of controls.

## How to use

### Setting up:

```
private static Gui gui;
...
Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);
var device = VeldridStartup.CreateGraphicsDevice(window);
gui = new Gui(window, device);
```

### Adding controls:

```
private static Control control;
private static Text text;
private static Button button;
private static TextBox textBox;
...

control = new Control()
{
    Size = new Vector2(500, 500),
    Position = new Vector2(5, 5),
    IsHoverable = false,
    IsClickable = false
};
control.Initialize();
control.SetCenterScreen();
gui.SceneGraph.Root.AddChild(control);

text = new Text("Text")
{
    Position = new Vector2(5, 5),
    Size = new Vector2(100, 34),
};
text.Initialize();
control.AddChild(text);

button = new Button("Button")
{
    Size = new Vector2(100, 34),
    Position = new Vector2(5, 40),
};
button.Initialize();
control.AddChild(button);

textBox = new TextBox()
{
    Size = new Vector2(160, 34),
    Position = new Vector2(5, 80),
    Text = "TextBox"
};
textBox.Initialize();
control.AddChild(textBox);
```

### Updating:

```
var snap = window.PumpEvents();
gui.Update(snap);
```

### Drawing:

```
gui.Draw();
```

### Cleaning up:

```
gui.Dispose();
```

## Thanks to

* [Veldrid](https://github.com/mellinoe/veldrid)
* [SixLabors](https://github.com/SixLabors)
* [OpenSAGE](https://github.com/OpenSAGE/OpenSAGE)
