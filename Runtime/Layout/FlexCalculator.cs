using Unity.Mathematics;

/// <summary>
/// Simple flex layout calculator, width and height can be swapped for column layout
/// </summary>
public struct FlexCalculator
{
    /// <summary>
    /// Maximum size of a line, if zero then no line limit
    /// </summary>
    readonly float lineWrap;

    /// <summary>
    /// Number of lines in the flex container
    /// </summary>
    int lineCount;

    /// <summary>
    /// Current length of the line
    /// </summary>
    float currentWidth;

    /// <summary>
    /// Total height of all previous lines
    /// </summary>
    float totalPreviousHeight;

    /// <summary>
    /// Height of the current line, calculated by the tallest item in the current line
    /// </summary>
    float lineHeight;

    /// <summary>
    /// Longest line in the flex container
    /// </summary>
    float widestLine;

    /// <summary>
    /// Gap between items
    /// </summary>
    float2 gap;

    public FlexCalculator(float wrap, float2 gap = default)
    {
        lineWrap = wrap;
        lineCount = 1;
        currentWidth = 0f;
        totalPreviousHeight = 0f;
        lineHeight = 0f;
        widestLine = 0f;
        this.gap = gap;
    }

    /// <summary>
    /// Add a value to the current line, returns position of the item
    /// </summary>
    /// <param name="directionLength"></param>
    /// <returns></returns>
    public float2 Add(float2 value)
    {
        var widthGap = currentWidth > 0 ? gap.x : 0f;
        var heightGap = totalPreviousHeight > 0 ? gap.y : 0f;
        var position = new float2(currentWidth + widthGap, totalPreviousHeight + heightGap);

        // Without wrapping just continuosly add to the current line
        if (lineWrap == 0f)
        {
            currentWidth += value.x;
            lineHeight = math.max(lineHeight, value.y);
            totalPreviousHeight = lineHeight;
            return new float2(position.x, 0f);
        }

        // Line wrapping
        if (currentWidth + value.x + widthGap > lineWrap)
        {
            lineCount++;
            totalPreviousHeight += lineHeight + heightGap;
            widestLine = math.max(widestLine, currentWidth);
            currentWidth = value.x;
            lineHeight = value.y;
            return new float2(0f, totalPreviousHeight + gap.y);
        }
        currentWidth += value.x + widthGap;
        lineHeight = math.max(lineHeight, value.y);
        return position;
    }

    public readonly int GetLineCount()
    {
        return lineCount;
    }

    public readonly float GetLongestLineLength()
    {
        return math.max(widestLine, currentWidth);
    }
}