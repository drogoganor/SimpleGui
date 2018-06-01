# SimpleGui for Veldrid

An amateur WIP user interface written for the [Veldrid](https://github.com/mellinoe/veldrid) library.

![Preview](https://github.com/drogoganor/SimpleGui/blob/master/images/Example.png)

It should be noted that Veldrid already does include bindings for [dear ImGui](https://github.com/ocornut/imgui) which is a reliable UI. But if that is not your style and you want to roll your own, forking this project might be for you. This project is experimental and probably most useful as an example of how to create your own UI library using Veldrid rather than for any serious use in an applicaton.

## About

SimpleGui currently supports only a limited set of simple controls:

* Control
* Label
* Checkbox
* Button
* TextBox (no selection)

Uses the [TextRender](https://github.com/drogoganor/TextRender) library which is a similar WIP of mine.

Gui configuration including color themes are configured in gui.json in the application directory.

There's no handling of control focus or overlapping controls yet.

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
InputTracker.UpdateFrameInput(snap);

gui.Update();
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
