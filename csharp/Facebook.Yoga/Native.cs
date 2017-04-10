/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Runtime.InteropServices;

namespace Facebook.Yoga
{
#if WINDOWS_UWP_ARM
    using YogaValueType = IntPtr;
#else
    using YogaValueType = YogaValue;
#endif

    internal static class Native
    {
#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
        private const string DllName = "__Internal";
#else
        private const string DllName = "yoga";
#endif

        internal class YGNodeHandle : SafeHandle
        {
#if (UNITY_IOS && !UNITY_EDITOR) || ENABLE_IL2CPP || __IOS__
            private GCHandle _managed;
#endif

            private YGNodeHandle() : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get
                {
                    return this.handle == IntPtr.Zero;
                }
            }

            protected override bool ReleaseHandle()
            {
#if (UNITY_IOS && !UNITY_EDITOR) || ENABLE_IL2CPP || __IOS__
                ReleaseManaged();
#endif
                Native.YGNodeFree(this.handle);
                GC.KeepAlive(this);
                return true;
            }

#if (UNITY_IOS && !UNITY_EDITOR) || ENABLE_IL2CPP || __IOS__
            public void SetContext(YogaNode node)
            {
                if (!_managed.IsAllocated)
                {
#if ENABLE_IL2CPP
                    // Weak causes 'GCHandle value belongs to a different domain' error
                    _managed = GCHandle.Alloc(node);
#else
                    _managed = GCHandle.Alloc(node, GCHandleType.Weak);
#endif
                    Native.YGNodeSetContext(this.handle, GCHandle.ToIntPtr(_managed));
                }
            }

            public void ReleaseManaged()
            {
                if (_managed.IsAllocated)
                {
                    _managed.Free();
                }
            }

            public static YogaNode GetManaged(IntPtr ygNodePtr)
            {
                var node =
                    GCHandle.FromIntPtr(Native.YGNodeGetContext(ygNodePtr)).Target as YogaNode;
                if (node == null)
                {
                    throw new InvalidOperationException("YogaNode is already deallocated");
                }

                return node;
            }
#endif
        }

        internal class YGConfigHandle : SafeHandle
        {
            private YGConfigHandle() : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get
                {
                    return this.handle == IntPtr.Zero;
                }
            }

            protected override bool ReleaseHandle()
            {
                Native.YGConfigFree(this.handle);
                GC.KeepAlive(this);
                return true;
            }
        }

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGInteropSetLogger(
            [MarshalAs(UnmanagedType.FunctionPtr)] YogaLogger.Func func);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YGNodeHandle YGNodeNew();

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YGNodeHandle YGNodeNewWithConfig(YGConfigHandle config);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeFree(IntPtr node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeReset(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YGConfigHandle YGConfigNew();

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGConfigFree(IntPtr node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int YGNodeGetInstanceCount();

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGConfigSetExperimentalFeatureEnabled(
            YGConfigHandle config,
            YogaExperimentalFeature feature,
            bool enabled);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool YGConfigIsExperimentalFeatureEnabled(
            YGConfigHandle config,
            YogaExperimentalFeature feature);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGConfigSetUseWebDefaults(
            YGConfigHandle config,
            bool useWebDefaults);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool YGConfigGetUseWebDefaults(YGConfigHandle config);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGConfigSetPointScaleFactor(
            YGConfigHandle config,
            float pixelsInPoint);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeInsertChild(
            YGNodeHandle node,
            YGNodeHandle child,
            uint index);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeRemoveChild(YGNodeHandle node, YGNodeHandle child);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeCalculateLayout(
            YGNodeHandle node,
            float availableWidth,
            float availableHeight,
            YogaDirection parentDirection);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeMarkDirty(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool YGNodeIsDirty(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodePrint(YGNodeHandle node, YogaPrintOptions options);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeCopyStyle(YGNodeHandle dstNode, YGNodeHandle srcNode);

#region YG_NODE_PROPERTY

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeSetMeasureFunc(
            YGNodeHandle node,
            [MarshalAs(UnmanagedType.FunctionPtr)] YogaMeasureFunc measureFunc);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeSetBaselineFunc(
            YGNodeHandle node,
            [MarshalAs(UnmanagedType.FunctionPtr)] YogaBaselineFunc baselineFunc);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeSetHasNewLayout(
            YGNodeHandle node,
            [MarshalAs(UnmanagedType.I1)] bool hasNewLayout);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool YGNodeGetHasNewLayout(YGNodeHandle node);

#endregion

#region YG_NODE_STYLE_PROPERTY

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetDirection(YGNodeHandle node, YogaDirection direction);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaDirection YGNodeStyleGetDirection(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexDirection(YGNodeHandle node, YogaFlexDirection flexDirection);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaFlexDirection YGNodeStyleGetFlexDirection(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetJustifyContent(YGNodeHandle node, YogaJustify justifyContent);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaJustify YGNodeStyleGetJustifyContent(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetAlignContent(YGNodeHandle node, YogaAlign alignContent);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaAlign YGNodeStyleGetAlignContent(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetAlignItems(YGNodeHandle node, YogaAlign alignItems);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaAlign YGNodeStyleGetAlignItems(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetAlignSelf(YGNodeHandle node, YogaAlign alignSelf);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaAlign YGNodeStyleGetAlignSelf(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetPositionType(YGNodeHandle node, YogaPositionType positionType);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaPositionType YGNodeStyleGetPositionType(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexWrap(YGNodeHandle node, YogaWrap flexWrap);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaWrap YGNodeStyleGetFlexWrap(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetOverflow(YGNodeHandle node, YogaOverflow flexWrap);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaOverflow YGNodeStyleGetOverflow(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetDisplay(YGNodeHandle node, YogaDisplay display);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaDisplay YGNodeStyleGetDisplay(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlex(YGNodeHandle node, float flex);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexGrow(YGNodeHandle node, float flexGrow);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeStyleGetFlexGrow(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexShrink(YGNodeHandle node, float flexShrink);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeStyleGetFlexShrink(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexBasis(YGNodeHandle node, float flexBasis);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexBasisPercent(YGNodeHandle node, float flexBasis);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetFlexBasisAuto(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetFlexBasis(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetWidth(YGNodeHandle node, float width);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetWidthPercent(YGNodeHandle node, float width);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetWidthAuto(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetWidth(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetHeight(YGNodeHandle node, float height);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetHeightPercent(YGNodeHandle node, float height);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetHeightAuto(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetHeight(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMinWidth(YGNodeHandle node, float minWidth);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMinWidthPercent(YGNodeHandle node, float minWidth);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetMinWidth(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMinHeight(YGNodeHandle node, float minHeight);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMinHeightPercent(YGNodeHandle node, float minHeight);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetMinHeight(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMaxWidth(YGNodeHandle node, float maxWidth);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMaxWidthPercent(YGNodeHandle node, float maxWidth);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetMaxWidth(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMaxHeight(YGNodeHandle node, float maxHeight);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMaxHeightPercent(YGNodeHandle node, float maxHeight);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetMaxHeight(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetAspectRatio(YGNodeHandle node, float aspectRatio);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeStyleGetAspectRatio(YGNodeHandle node);

#endregion

#region YG_NODE_STYLE_EDGE_PROPERTY

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetPosition(YGNodeHandle node, YogaEdge edge, float position);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetPositionPercent(YGNodeHandle node, YogaEdge edge, float position);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetPosition(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMargin(YGNodeHandle node, YogaEdge edge, float margin);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMarginPercent(YGNodeHandle node, YogaEdge edge, float margin);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetMarginAuto(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetMargin(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetPadding(YGNodeHandle node, YogaEdge edge, float padding);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetPaddingPercent(YGNodeHandle node, YogaEdge edge, float padding);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaValueType YGNodeStyleGetPadding(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeStyleSetBorder(YGNodeHandle node, YogaEdge edge, float border);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeStyleGetBorder(YGNodeHandle node, YogaEdge edge);

#endregion

#region YG_NODE_LAYOUT_PROPERTY

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetLeft(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetTop(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetRight(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetBottom(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetWidth(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetHeight(YGNodeHandle node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetMargin(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern float YGNodeLayoutGetPadding(YGNodeHandle node, YogaEdge edge);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern YogaDirection YGNodeLayoutGetDirection(YGNodeHandle node);

#endregion

#region AOT

#if (UNITY_IOS && !UNITY_EDITOR) || ENABLE_IL2CPP || __IOS__
        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr YGNodeGetContext(IntPtr node);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void YGNodeSetContext(IntPtr node, IntPtr managed);
#endif

#endregion
    }
}
