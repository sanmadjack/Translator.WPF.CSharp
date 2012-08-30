using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Controls.Ribbon;
namespace Translator.WPF {
    public class TranslationHelpers {


        private static bool objectIsOfType(object obj, Type type) {
            Type check = obj.GetType();
            return check.Equals(type)||check.IsSubclassOf(type);
        }

        #region Translating methods
        public static void translateWindow(Window window) {
            translateTitle(window);
            translateRecursively(window.Content as UIElement);
        }


        private static void translateRecursively(UIElement obj) {
            if (obj is FrameworkElement) {
                FrameworkElement fe = obj as FrameworkElement;
                if (fe.ContextMenu != null) {
                    foreach (MenuItem item in fe.ContextMenu.Items) {
                        translateMenuItem(item);
                    }
                }
            }

            if (obj == null ||
                  obj is ProgressBar ||
                  obj is ComboBox ||
                  obj is Image ||
                  obj is TreeView ||
                  obj is System.Windows.Shapes.Shape ||
                obj is PasswordBox ||
                obj is GridSplitter ||
                obj is ResizeGrip) {
            } else if (objectIsOfType(obj, typeof(TextBox))) {
                if (objectIsOfType(obj,typeof(RibbonTextBox))) {
                    translateLabel(obj as RibbonTextBox);
                }
            } else if (obj is ItemsControl) {
                ItemsControl items = obj as ItemsControl;
                foreach (object item in items.Items) {
                    if(item.GetType().IsSubclassOf(typeof(UIElement)))
                        translateRecursively(item as UIElement);
                }
                if (obj is HeaderedItemsControl) {
                    if (obj is MenuItem) {
                        translateMenuItem(obj as MenuItem);
                    } else {
                        translateHeader(obj as HeaderedItemsControl);
                    }
                }
                if (obj is ListView) {
                    ListView list = obj as ListView;
                    if (list.View != null) {
                        GridView view = list.View as GridView;
                        if (view.Columns != null) {
                            foreach (GridViewColumn col in view.Columns) {
                                translateColumnHeader(col);
                            }
                        }
                    }
                }
                if (obj is RibbonMenuButton) {
                    translateLabel(obj as RibbonMenuButton);
                }
            } else if (obj is Panel) {
                Panel grid = obj as Panel;
                foreach (UIElement elem in grid.Children) {
                    translateRecursively(elem);
                }
            } else if (obj is StatusBar) {
                StatusBar bar = obj as StatusBar;
                foreach (UIElement element in bar.Items) {
                    translateRecursively(element);
                }
            } else if (obj is TabControl) {
                TabControl tabs = obj as TabControl;
                foreach (TabItem item in tabs.Items) {
                    translateRecursively(item);
                }
            } else if (obj is StatusBarItem) {
                StatusBarItem item = obj as StatusBarItem;
                translateRecursively(item.Content as UIElement);
            } else if (obj is RibbonCheckBox) {
                translateLabel(obj as RibbonCheckBox);
            } else if (obj is ContentControl) {
                if (obj is HeaderedContentControl) {
                    HeaderedContentControl head = obj as HeaderedContentControl;
                    translateHeader(head);
                    translateRecursively(head.Content as UIElement);
                } else if (obj is RibbonButton) {
                    translateLabel(obj as RibbonButton);
                } else if (obj is RibbonToggleButton) {
                    translateLabel(obj as RibbonToggleButton);
                } else if (obj is Button) {
                    translateButton(obj as Button);
                } else {
                    translateContent(obj as ContentControl);
                }

            } else if (objectIsOfType(obj,typeof(TextBlock))) {
                translateText(obj as TextBlock);
            } else if (objectIsOfType(obj, typeof(Decorator))) {
                translateRecursively((obj as Decorator).Child);
            } else if (objectIsOfType(obj, typeof(UserControl))) {
                translateControl(obj as UserControl);
            } else {
                throw new Exception("Can't translate object " + obj.GetType().ToString());
            }
        }

        public static void translate(UIElement obj, string name, params string[] variables) {
            name = "$" + name;
            if (obj == null ||
                  obj is ProgressBar ||
                  obj is ComboBox ||
                  obj is Image ||
                  obj is TreeView ||
                obj is PasswordBox) {
            } else if(obj is TextBox) {
                if (obj is RibbonTextBox || obj.GetType().IsSubclassOf(typeof(RibbonTextBox))) {
                    translateLabel(obj as RibbonTextBox, name, variables);
                }
            } else if (obj is TextBlock) {
                translateText(obj as TextBlock, name, variables);
            } else if (obj is ContentControl) {
                if (obj is Window) {
                    translateTitle(obj as Window, name, variables);
                } else if (obj is HeaderedContentControl) {
                    translateHeader(obj as HeaderedContentControl,name,variables);
                } else if (obj is RibbonButton) {
                    translateLabel(obj as RibbonButton,name,variables);
                } else {
                    translateContent(obj as ContentControl,name,variables);
                }

            } else {
                throw new Exception("Can't translate object " + obj.GetType().ToString());
            }
        }

        #region translate Ribbon Labels
        
        private static void translateLabel(RibbonMenuButton button) {
            if (button.Label == null)
                return;
            string string_title = button.Label.ToString();
            translateLabel(button, string_title);
        }

        private static void translateLabel(RibbonMenuButton button, string name, params string[] variables) {
            StringCollection str = Strings.getInterfaceString(name);
            if (str[StringType.Label].HasHotKey) {
                button.KeyTip = str[StringType.Label].hotkey;
            }

            button.Label = str[StringType.Label].interpret(variables);

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret(variables);
            }
        }

        private static void translateLabel(RibbonCheckBox button) {
            string name = button.Label.ToString();


            StringCollection str = Strings.getInterfaceString(name);
            if (str[StringType.Label].HasHotKey) {
                button.KeyTip = str[StringType.Label].hotkey;
            }

            button.Label = str[StringType.Label].interpret();

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret();
            }
        }
        private static void translateTitle(Window window, string name, params string[] variables) {
            window.Title = Strings.getInterfaceString(name)[StringType.Label].interpret(variables);
        }


        private static void translateTitle(Window window) {
            string string_title = window.Title.ToString();
            translateTitle(window, string_title);
        }
        private static void translateLabel(RibbonTextBox button) {
            if (button.Label == null)
                return;
            string string_title = button.Label.ToString();
            translateLabel(button, string_title);
        }
        private static void translateLabel(RibbonTextBox button, string name, params string[] variables) {
            StringCollection str = Strings.getInterfaceString(name);
            if (str[StringType.Label].HasHotKey) {
                button.KeyTip = str[StringType.Label].hotkey;
            }

            button.Label = str[StringType.Label].interpret(variables);

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret(variables);
            }
        }

        private static void translateLabel(RibbonButton button) {
            if (button.Label == null)
                return;
            string string_title = button.Label.ToString();
            translateLabel(button, string_title);
        }
        private static void translateLabel(RibbonButton button, string name, params string[] variables) {
            StringCollection str = Strings.getInterfaceString(name);
            if (str[StringType.Label].HasHotKey) {
                button.KeyTip = str[StringType.Label].hotkey;
            }

            button.Label = str[StringType.Label].interpret(variables);

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret(variables);
            }
        }
        private static void translateLabel(RibbonToggleButton button) {
            if (button.Label == null)
                return;
            string string_title = button.Label.ToString();
            translateLabel(button, string_title);
        }
        private static void translateLabel(RibbonToggleButton button, string name, params string[] variables) {
            StringCollection str = Strings.getInterfaceString(name);
            if (str[StringType.Label].HasHotKey) {
                button.KeyTip = str[StringType.Label].hotkey;
            }

            button.Label = str[StringType.Label].interpret(variables);

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret(variables);
            }
        }
        #endregion

        private static TranslateableString translateText(TextBlock text) {
            string string_title = text.Text.ToString();
            if (!string_title.StartsWith("$"))
                return null;
            return translateText(text, string_title);
        }
        private static TranslateableString translateText(TextBlock text, string name, params string[] variables) {
            TranslateableString str = Strings.getInterfaceString(name)[StringType.Label];
            text.Text = str.interpret(variables);
            return str;
        }
        private static void translateMenuItem(MenuItem item) {
            string string_title = item.Header.ToString();
            if (string_title != "") {
                StringCollection str = Strings.getInterfaceString(string_title);

                item.Header = str[StringType.Label].interpret();

                if (str.ContainsKey(StringType.ToolTip)) {
                    item.ToolTip = str[StringType.ToolTip].interpret();
                }
            }

        }

        private static void translateButton(Button control) {
            if (control.Content is string) {
                translateButton(control, control.Content as string);
            } else {
                translateContent(control);
            }
        }
        private static void translateButton(Button button, string name, params string[] variables) {
            StringCollection str = Strings.getInterfaceString(name);
            //if (str[StringType.Label].HasHotKey) {
            //    button.key = str[StringType.Label].hotkey;
            //}

            if (str.ContainsKey(StringType.ToolTip)) {
                button.ToolTip = str[StringType.ToolTip].interpret(variables);
            }

            translateContent(button, name);
        }

        private static string translateContent(ContentControl control) {
            if (control.Content == null)
                return null;

            if (control.Content is UIElement) {
                translateRecursively(control.Content as UIElement);
                return null;
            }

            string string_title = control.Content.ToString();

            translateContent(control,string_title);

            return string_title;
        }

        private static void translateContent(ContentControl control, string name, params string[] variables) {
            TranslateableString str = Strings.getInterfaceString(name)[StringType.Label];
                control.Content = str.interpret(variables);
        }

        #region Header translators
        private static void translateHeader(HeaderedContentControl control) {
            if (control.Header == null)
                return;

            string string_title = control.Header.ToString();
            translateHeader(control, string_title);
        }
        private static void translateHeader(HeaderedContentControl control, string name, params string[] variables) {
            TranslateableString str = Strings.getInterfaceString(name)[StringType.Label];
            control.Header = str.interpret(variables);
        }
        private static void translateHeader(HeaderedItemsControl control) {
            if (control.Header == null) {
                control.Header = Strings.getInterfaceString(null)[StringType.Label].interpret();
                return;
            }
            string string_title = control.Header.ToString();

            TranslateableString str = Strings.getInterfaceString(string_title)[StringType.Label];
            if (control is RibbonTab) {
                RibbonTab tab = control as RibbonTab;
                if (str.HasHotKey) {
                    tab.KeyTip = str.hotkey;
                }
            }
            control.Header = str.interpret();
        }
        #endregion

        private static void translateColumnHeader(GridViewColumn control) {
            if (control.Header == null) {
                control.Header = Strings.getInterfaceString(null)[StringType.Label].interpret();
                return;
            }
            string string_title = control.Header.ToString();
            control.Header = Strings.getInterfaceString(string_title)[StringType.Label].interpret();
        }

        private static void translateControl(UserControl control) {
            translateRecursively(control.Content as UIElement);
        }

        #endregion
    }
}
