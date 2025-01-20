﻿namespace Cherris;

public enum MouseCursorCode
{
    /// <summary>
    /// Default pointer shape
    /// </summary>
    Default = 0,

    /// <summary>
    /// Arrow shape
    /// </summary>
    Arrow = 1,

    /// <summary>
    /// Text writing cursor shape
    /// </summary>
    IBeam = 2,

    /// <summary>
    /// Cross shape
    /// </summary>
    Crosshair = 3,

    /// <summary>
    /// Pointing hand cursor
    /// </summary>
    PointingHand = 4,

    /// <summary>
    /// Horizontal resize/move arrow shape
    /// </summary>
    ResizeHorizontal = 5,

    /// <summary>
    /// Vertical resize/move arrow shape
    /// </summary>
    ResizeVertical = 6,

    /// <summary>
    /// Top-left to bottom-right diagonal resize/move arrow shape
    /// </summary>
    ResizeBottomLeftToTopRight = 7,

    /// <summary>
    /// The top-right to bottom-left diagonal resize/move arrow shape
    /// </summary>
    ResizeTopLeftToBottomRight = 8,

    /// <summary>
    /// The omnidirectional resize/move cursor shape
    /// </summary>
    ResizeAll = 9,

    /// <summary>
    /// The operation-not-allowed shape
    /// </summary>
    NotAllowed = 10
}