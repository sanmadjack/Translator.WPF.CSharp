﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Controls.Ribbon;
namespace Translator.WPF {
    public class TranslationHelpers {
        #region TranslatedMessageBoxes
        public static bool askTranslatedQuestion(ITranslateableWindow window, String string_name, params string[] variables) {
            StringCollection mes = Strings.getStrings(string_name);
            return window.displayQuestion(mes[StringType.Title].interpret(variables),
                mes[StringType.Message].interpret(variables));
        }
        public static bool showTranslatedWarning(ITranslateableWindow window, String string_name, params string[] variables) {
            StringCollection mes = Strings.getStrings(string_name);
            return window.displayWarning(mes[StringType.Title].interpret(variables),
                mes[StringType.Message].interpret(variables));
        }
        public static bool showTranslatedError(ITranslateableWindow window, String string_name, params string[] variables) {
            return showTranslatedError(window, string_name, null, variables);
        }
        public static bool showTranslatedError(ITranslateableWindow window, String string_name, Exception ex, params string[] variables) {
            StringCollection mes = Strings.getStrings(string_name);
            return window.displayError(mes[StringType.Title].interpret(variables),
                mes[StringType.Message].interpret(variables), ex);
        }
        public static bool showTranslatedInfo(ITranslateableWindow window, String string_name, params string[] variables) {
            StringCollection mes = Strings.getStrings(string_name);
            return window.displayInfo(mes[StringType.Title].interpret(variables),
                mes[StringType.Message].interpret(variables));
        }
        #endregion


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
                obj is TextBox ||
                  obj is ProgressBar ||
                  obj is ComboBox ||
                  obj is Image ||
                  obj is TreeView ||
                obj is PasswordBox) {
            } else if (obj is ItemsControl) {
                ItemsControl items = obj as ItemsControl;
                foreach (UIElement item in items.Items) {
                    translateRecursively(item);
                }
                if (obj is HeaderedItemsControl) {
                    translateHeader(obj as HeaderedItemsControl);
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
                } else {
                    translateContent(obj as ContentControl);
                }

            } else if (obj is TextBlock) {
                translateText(obj as TextBlock);
            } else if (obj is UserControl) {
                translateControl(obj as UserControl);
            } else {
                throw new Exception("Can't translate object " + obj.GetType().ToString());
            }
        }

        public static void translate(UIElement obj, string name, params string[] variables) {
            if (obj == null ||
                obj is TextBox ||
                  obj is ProgressBar ||
                  obj is ComboBox ||
                  obj is Image ||
                  obj is TreeView ||
                obj is PasswordBox) {
            } else if (obj is ContentControl) {
                if (obj is HeaderedContentControl) {
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
            string string_title = button.Label.ToString();
            button.Label = Strings.getInterfaceString(string_title)[StringType.Label].interpret();

        }
        private static void translateTitle(Window window) {
            string string_title = window.Title.ToString();
            window.Title = Strings.getInterfaceString(string_title)[StringType.Label].interpret();
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
        #endregion

        private static TranslateableString translateText(TextBlock text) {
            string string_title = text.Text.ToString();
            TranslateableString str = Strings.getInterfaceString(string_title)[StringType.Label];
            text.Text = str.interpret();
            return str;
        }
        private static void translateMenuItem(MenuItem item) {
            string string_title = item.Header.ToString();
            if (string_title != "") {
                item.Header = Strings.getInterfaceString(string_title);
            }
        }

        private static void translateContent(ContentControl control) {
            if (control.Content == null)
                return;
            if (control.Content is TextBlock) {
                translateText(control.Content as TextBlock);
                return;
            }

                string string_title = control.Content.ToString();

            translateContent(control,string_title);
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