﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using O2.FluentSharp;
using O2.DotNetWrappers.DotNet;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE80;
//O2Ref:Microsoft.CSharp.dll
//O2Ref:Microsoft.VisualStudio.CommandBars.dll
//O2File:VisualStudio_2010_ExtensionMethods.cs

namespace O2.DotNetWrappers.ExtensionMethods
{
    public static class VS_Menus_ExtensionMethods
    {
        public static CommandBarPopup add_TopMenu(this DTE2 dte, string text = "New Top Menu", string addAfterMenu = "Help")
        {
            var o2Timer = new O2Timer("Adding TopMenu").start();
            if (dte.isNull())
            {
                "[VS_Menus_ExtensionMethods][add_TopMenu] DTE object is null, so can't create Top Menu".error();
                return null;
            }
            var existingMenu = dte.get_Menu(text);
            if (existingMenu.notNull())
            {
                "[VS_Menus_ExtensionMethods] add_TopMenu: there was already a menu called '{0}' so returning it".debug(text);
                return existingMenu;
            }
            "[VS_Menus_ExtensionMethods] Creating new Top Menu called: {0}".info(text);
            //dynamic commandBars = dte.CommandBars;
            //var menuCommandBar = commandBars["MenuBar"];  
            //get_CommandBarMenu
            //var position = (commandBars[addAfterMenu].Parent as CommandBarControl).Index;
            
            //The two lines above (using dynamic) has much worse performace then the ones below (from 1.8 sec to .1 sec)
            var menuCommandBar = dte.get_CommandBar("MenuBar");
            var position = dte.get_CommandBarMenu(addAfterMenu).Index;
            

            var newMenu = (CommandBarPopup)menuCommandBar.Controls.Add(MsoControlType.msoControlPopup,
                                                                       System.Type.Missing,
                                                                       System.Type.Missing,
                                                                       ++position,
                                                                       true);
            newMenu.Caption = text;
            newMenu.Enabled = true;
            o2Timer.stop();
            return newMenu;
        }

        public static CommandBarPopup add(this CommandBarPopup topMenu, string text, Action onClick)
        {
            return topMenu.add_Menu_Button(text, onClick);
        }
        public static CommandBarPopup add_Menu_Button(this CommandBarPopup topMenu, string text = "New Button", Action onClick = null, int before = 0)
        {
            if (topMenu.isNull())
            {
                "[VS_Menus_ExtensionMethods][add_Menu_Button] provided topMenu was null so can create menu button: {0}".error(text);
                return null;
            }
            if (before == 0)            
                before = topMenu.Controls.Count + 1; // to put it at the end
            var button = (CommandBarButton)topMenu.Controls.Add(MsoControlType.msoControlButton, // type
                                                                 System.Type.Missing, 			  // id
                                                                 System.Type.Missing, 			  // parameter
                                                                 before,						  // before
                                                                 false);						  // temporary

            button.Click += (CommandBarButton sender, ref bool CancelDefault) => onClick.invoke();
            button.Caption = text;
            button.Enabled = true;
            return topMenu;
        }


        public static CommandBar get_CommandBar(this DTE2 dte2, string commandBarName)
        {
            try
            {
                var commandBars = (CommandBars)dte2.CommandBars;
                return commandBars[commandBarName];
            }
            catch
            {
                return null;
            }
        } 

        public static CommandBarControl get_CommandBarMenu(this DTE2 dte2, string menuName)
        {
            return dte2.get_CommandBarMenu<CommandBarControl>(menuName);
        }

        public static T get_CommandBarMenu<T>(this DTE2 dte2, string menuName)
            where T : CommandBarControl
        {
            var menuBarCommandBar = dte2.get_CommandBar("MenuBar");
            //if (menuBarCommandBar.Controls.ContainsKey(menuName))
            try
            {
                var menu = menuBarCommandBar.Controls[menuName];
                if (menu is T)
                    return (T)menu;
            }
            catch { }
            return default(T);
        }

        public static CommandBarPopup get_CommandBarPopup(this DTE2 dte2, string menuName)
        {
            return dte2.get_CommandBarMenu<CommandBarPopup>(menuName);
        }

        public static CommandBarPopup get_Menu(this DTE2 dte2, string menuName)
        {
            return dte2.get_CommandBarPopup(menuName);
        }
    }
}