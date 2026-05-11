using System;
using UnityEngine;

namespace KopruBekcisi.Gameplay.Inspection
{
    public class InspectionController : MonoBehaviour
    {
        public InspectionTool ActiveTool { get; private set; } = InspectionTool.None;

        public event Action<InspectionTool> OnToolChanged;

        public void Toggle(InspectionTool tool)
        {
            ActiveTool = (ActiveTool == tool) ? InspectionTool.None : tool;
            OnToolChanged?.Invoke(ActiveTool);
        }

        public void SetTool(InspectionTool tool)
        {
            if (ActiveTool == tool) return;
            ActiveTool = tool;
            OnToolChanged?.Invoke(ActiveTool);
        }

        public void ClearTool()
        {
            if (ActiveTool == InspectionTool.None) return;
            ActiveTool = InspectionTool.None;
            OnToolChanged?.Invoke(ActiveTool);
        }
    }
}
