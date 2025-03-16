# ECS UI

Unity UI framework for ECS

## Workflow

Write in XML, style with tags.

There are only two type of elements: `<block>` and `<inline>`

```
<block>
    Hello, world!
</block>
```

### Available styles

| Name           | Component            | Values | Description |
|----------------|----------------------|--------|-------------|
| `position`     | `UILayoutPosition`   |        |             |
| `layout`       | `UILayoutType`       |        |             |
| `width`        | `UIStyleSize`        |        |             |
| `height`       | `UIStyleSize`        |        |             |
| `background`   | `UIStyleBackground`  |        |             |
| `margin`       | `UIStyleMargin`      |        |             |
| `padding`      | `UIStylePadding`     |        |             |
| `aspect-ratio` | `UIStyleAspectRatio` |        |             |

### Interactivity

Reference an element using IDs.

```
<block>
    <block id="my-element" />
</block>
```

```C#
using Unity.Entities;
using ElfenLabs.UI;

public partial struct UITestSystem : ISystem 
{
    Entity myElement;
    
    void OnCreate(ref SystemState state)
    {
        myElement = UIUtility.GetElement(ref state, "my-element");
    }
} 
```

## Example

```
<block class="root">
  <block class="container">
    <block class="item">one</block>
    <block class="item">one</block>
    <block class="item">one two three four five</block>
    <block class="item">one two three four five six seven eight nine</block></block>
  <block class="second">
    dfsdf
  </block>
</block>
```

