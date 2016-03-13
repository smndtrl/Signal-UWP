﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.Resources
{
    public class ExtendedVisualStateManager : VisualStateManager
    {
        protected override bool GoToStateCore(Control control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if ((group == null) || (state == null))
            {
                return false;
            }

            if (control == null)
            {
                control = new ContentControl();
            }

            return base.GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);
        }

        public static bool GoToElementState(FrameworkElement element, string stateName, bool useTransitions)
        {
            var root = FindNearestStatefulFrameworkElement(element);

            var customVisualStateManager = VisualStateManager.GetCustomVisualStateManager(root) as ExtendedVisualStateManager;

            return ((customVisualStateManager != null) && customVisualStateManager.GoToStateInternal(root, stateName, useTransitions));
        }

        private static FrameworkElement FindNearestStatefulFrameworkElement(FrameworkElement element)
        {
            while (element != null && VisualStateManager.GetCustomVisualStateManager(element) == null)
            {
                element = element.Parent as FrameworkElement;
            }

            return element;
        }

        private bool GoToStateInternal(FrameworkElement stateGroupsRoot, string stateName, bool useTransitions)
        {
            VisualStateGroup group;
            VisualState state;

            return (TryGetState(stateGroupsRoot, stateName, out group, out state) && this.GoToStateCore(null, stateGroupsRoot, stateName, group, state, useTransitions));
        }

        private static bool TryGetState(FrameworkElement element, string stateName, out VisualStateGroup group, out VisualState state)
        {
            group = null;
            state = null;

            foreach (VisualStateGroup group2 in VisualStateManager.GetVisualStateGroups(element))
            {
                foreach (VisualState state2 in group2.States)
                {
                    if (state2.Name == stateName)
                    {
                        group = group2;
                        state = state2;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
