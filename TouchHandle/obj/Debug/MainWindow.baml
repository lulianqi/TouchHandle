   M S B A M L   `   `   `  ����  TouchHandle    TouchHandle.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  MicroControl��5   	   $	��315��6   $	��590��6+    � 
ResizeMode$ CanMinimize=�6N    � Icon$ myResources/handle.ico��6g   .��5      � -   5        $��	Grid_Main��5      .+�5	   
   �� -   5         ��Name$ key_up��5	      $
��Left=�6!   $��60��6<   $��60��6H   $��	80,13,0,0q�6S   $	��Top=�6f   ��6~   Z� ��6�   Z� .��5
      �� $��myResources/key_up.png��6   5      �� 5   
   -   5        $ key_ok��5      $
��Left=�6!   $��60��6<   $��60��6H   $��	80,78,0,0q�6S   $	��Top=�6f   ��6~   Z� ��6�   Z� .��5      �� $��myResources/key_ok.png��6   5      �� 5   
   -   5        $ key_down��5      $
��Left=�6#   $��60��6>   $��60��6J   $��
80,143,0,0q�6U   $	��Top=�6i   ��6�   Z� ��6�   Z� .��5      �� $��myResources/key_down.png��6   5      �� 5   
   -   5        $ key_left��5      $
��Left=�6#   $��60��6>   $��60��6J   $��	15,78,0,0q�6U   $	��Top=�6h   ��6�   Z� ��6�   Z� .��5      �� $��myResources/key_left.png��6   5      �� 5   
   -   5        $ 	key_right��5      $
��Left=�6$   $��60��6?   $��60��6K   $��
145,78,0,0q�6V   $	��Top=�6j   ��6�   Z� ��6�   Z� .��5      �� $��myResources/key_right.png��6   5      �� 5   
   -   5        $ key_A��5      $
��Left=�6    $��60��6;   $��60��6G   $��
225,78,0,0q�6R   $	��Top=�6f   ��6~   Z� ��6�   Z� .��5      �� $��myResources/key_A.png��6   5      �� 5   
   -	   5        $ key_C��5      $
��Left=�6    $��60��6;   $��60��6G   $��225,143,0,0q�6R   $	��Top=�6g   ��6   Z� ��6�   Z� .��5      �� $��myResources/key_C.png��6   5      �� 5   
   -
   5        $ key_B��5      $
��Left=�6    $��60��6;   $��60��6G   $��
290,78,0,0q�6R   $	��Top=�6f   ��6   Z� ��6�   Z� .��5      �� $��myResources/key_B.png��6   5       �� 5!   
   -   5        $ key_D��5!      $
��Left=�6    $��60��6;   $��60��6G   $��290,143,0,0q�6R   $	��Top=�6g   ��6�   Z� ��6�   Z� .��5"      �� $��myResources/key_D.png��6   5#      �� 5$   
   -   5         ��Name$ lv_keyVaule��5$      $
��Left=�6(   $	��179��6C   $��
377,40,0,0q�6P   $	��Top=�6d   $	��184��6|   ��6�   ��  �� View 5%      �� 5&      .P�5'      �� $��按键��6&    �� Width$ 75��62    �� DisplayMemberBinding 6=   ��  �� Path$	 Key��� 5(      $��键值��6&   $ 80��62    6=   �� $ Value�5)      5+      �� 5,   
   -   5        $ bt_startHandel��5,      $
��Left=�6)   $��60��6D   $��60��6P   $��508,219,0,0q�6[   $	��Top=�6p   ��6�   Z� ��6�   Z� .��5-      �� $!��myResources/startHandle.ico��6   5.      �� 5/   
   -   5        $��SliderSizeChange��5/      $
��Left=�6)   $��384,240,0,0q�6D   $	��Top=�6Y   	 J� RenderTransformOrigin$	 -0.75,-0.722&�6q   $	��119��6�   
 � Maximum$
 2Q�6�    � SmallChange$	 0.1Q�6�    � Minimum$	 0.5Q�6�   $��36��6�    �� TickPlacement$ BottomRight=�6   �� TickFrequency$	 0.5Q�6$   � Value$ 1Q�68  �� 50   
   -   5        $��ComboBox_KeyMapList��50      $
��Left=�6.   $��
377,10,0,0q�6I   $	��Top=�6]   $	��154��6u   $��21��6�   �� 51   
   -   5        $ bt_editNowName��51      $
��Left=�6)   $��30��6D   $��30��6P   $��	534,5,0,0q�6[   $	��Top=�6n   ��6�   Z� ��6�   Z� .��52      �� -   5        $��Image_editNowName��52      $#��myResources/nameList_edit.png��6-   53      �� 54   
   -   5        $
��Left=�54      $��
10,229,0,0q�6,   $	��Top=�6@   $��50��6X   $��50��6c   ��6p   Z� ��6�   Z� .��55      �� $!��myResources/Suggestions.png��6   56      57      58      