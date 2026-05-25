# This file is in the public domain. Feel free to modify it as a basis
# for your own screens.

##############################################################################
# Say
#
# Screen that's used to display adv-mode dialogue.
# http://www.renpy.org/doc/html/screen_special.html#say
screen say:
    zorder -1000

    # Defaults for side_image and two_window
    default side_image = None
    default two_window = True
    
    window:
        style "menu_window"
        xalign 0.5
        yalign 0.5

        vbox:
            style "menu"
            spacing 2


    # Decide if we want to use the one-window or two-window variant.
    if not two_window:

        # The one window variant.
        window:
            id "window"

            has vbox:
                style "say_vbox"

            if who:
                text who id "who"

            text what id "what"

    else:

        # The two window variant.
        vbox:
            style "say_two_window_vbox"

            if who:
                window:
                    style "say_who_window"

                    text who:
                        id "who"

            window:
                id "window"

                has vbox:
                    style "say_vbox"

                text what id "what"

    # If there's a side image, display it above the text.
    if side_image:
        add side_image
        
    else:
        add SideImage() xalign 0.034 yalign 0.99

    # Use the quick menu.
    use quick_menu


##############################################################################
# Choice
#
# Screen that's used to display in-game menus.
# http://www.renpy.org/doc/html/screen_special.html#choice

screen choice:

    window:
        style "menu_window"
        xalign 0.5
        yalign 0.5

        vbox:
            style "menu"
            spacing 2

            for caption, action, chosen in items:

                if action:

                    button:
                        action action
                        style "menu_choice_button"

                        text caption style "menu_choice"

                else:
                    text caption style "menu_caption"

init -2:
    $ config.narrator_menu = True

    style menu_window is default

    style menu_choice is button_text:
        clear

    style menu_choice_button is button:
        xminimum int(config.screen_width * 0.75)
        xmaximum int(config.screen_width * 0.75)


##############################################################################
# Input
#
# Screen that's used to display renpy.input()
# http://www.renpy.org/doc/html/screen_special.html#input

screen input:

    window style "input_window":
        has vbox

        text prompt style "input_prompt"
        input id "input" style "input_text"

    use quick_menu

##############################################################################
# Nvl
#
# Screen used for nvl-mode dialogue and menus.
# http://www.renpy.org/doc/html/screen_special.html#nvl

screen nvl:

    window:
        style "nvl_window"

        has vbox:
            style "nvl_vbox"

        # Display dialogue.
        for who, what, who_id, what_id, window_id in dialogue:
            window:
                id window_id

                has hbox:
                    spacing 10

                if who is not None:
                    text who id who_id

                text what id what_id

        # Display a menu, if given.
        if items:

            vbox:
                id "menu"

                for caption, action, chosen in items:

                    if action:

                        button:
                            style "nvl_menu_choice_button"
                            action action

                            text caption style "nvl_menu_choice"

                    else:

                        text caption style "nvl_dialogue"

    add SideImage() xalign 0.0 yalign 1.0

    use quick_menu

##############################################################################
# Main Menu
#
# Screen that's used to display the main menu, when Ren'Py first starts
# http://www.renpy.org/doc/html/screen_special.html#main-menu

screen main_menu():

    # This ensures that any other menu screen is replaced.
    tag menu

    imagemap:
        ground "gui/mainmenuidle.jpg"
        idle "gui/mainmenuidle.jpg"
        hover "gui/mainmenuhover.jpg"

        hotspot (72, 119, 60, 26) action Start() 
        hotspot (148, 119, 60, 26) action ShowMenu("load") 
        hotspot (223, 119, 100, 26) action ShowMenu("preferences") 
        hotspot (338, 119, 60, 26) action Quit() 

init -2 python:

    # Make all the main menu buttons be the same size.
    style.mm_button.size_group = "mm"



##############################################################################
# Navigation
#
# Screen that's included in other screens to display the game menu
# navigation and background.
# http://www.renpy.org/doc/html/screen_special.html#navigation
screen navigation:

    # The background of the game menu.
    window:
        style "gm_root"

    # The various buttons.
    frame:
        style_group "gm_nav"
        xalign .98
        yalign .98

        has vbox

        textbutton _("Return") action Return()
        textbutton _("Preferences") action ShowMenu("preferences")
        textbutton _("Save Game") action ShowMenu("save")
        textbutton _("Load Game") action ShowMenu("load")
        textbutton _("Main Menu") action MainMenu()
        textbutton _("Help") action Help()
        textbutton _("Quit") action Quit()

init -2:

    # Make all game menu navigation buttons the same size.
    style gm_nav_button:
        size_group "gm_nav"


##############################################################################
# Save, Load
#
# Screens that allow the user to save and load the game.
# http://www.renpy.org/doc/html/screen_special.html#save
# http://www.renpy.org/doc/html/screen_special.html#load

# Since saving and loading are so similar, we combine them into
# a single screen, file_picker. We then use the file_picker screen
# from simple load and save screens.

screen save:
    
    tag menu

    imagemap:
        
        ground "gui/save-idle.png"
        idle "gui/save-idle.png"
        hover "gui/save-hover.png"
        cache False
                       
        use file_picker
      
      
screen load:
   
    tag menu

    if main_menu:
        # window:
        #     style "mm_root"
        add "images/backgrounds/Background-by-ansimuz.png"

    imagemap:
        ground "gui/load-idle.png"
        idle "gui/load-idle.png"
        hover "gui/load-hover.png"
        cache False
                          
        use file_picker          
        
        
        
#####SAVE/LOAD FILE PICKER##############
screen file_picker:

    hotspot (7,7,28,28) action Return() 

    hotspot (70, 96, 105, 42) clicked FileAction(1):
        use load_save_slot(number=1)

    hotspot (192, 96, 105, 42) clicked FileAction(2):
        use load_save_slot(number=2)

    hotspot (314, 96, 105, 42) clicked FileAction(3):
        use load_save_slot(number=3)        
        
    hotspot (70, 152, 105, 42) clicked FileAction(4):
        use load_save_slot(number=4)

    hotspot (192, 152, 105, 42) clicked FileAction(5):
        use load_save_slot(number=5)

    hotspot (314, 152, 105, 42) clicked FileAction(6):
        use load_save_slot(number=6)
        
        
    hotspot (70, 207, 105, 42) clicked FileAction(7):
        use load_save_slot(number=7)

    hotspot (192, 207, 105, 42) clicked FileAction(8):
        use load_save_slot(number=8)

    hotspot (314, 207, 105, 42) clicked FileAction(9):
        use load_save_slot(number=9)    
    
    hotspot (0,261,96,60) action ShowMenu("preferences")
    hotspot (387,261,96,60) action ShowMenu("load")
    hotspot (97,261,96,60) action ShowMenu("credits")

    if not main_menu:
        hotspot (195,261,96,60) action ShowMenu("character_customization_screen")
        hotspot (289,261,96,60) action ShowMenu("save")
    

screen load_save_slot:

    $ file_text = "% 2s. %s\n%s" % (
                        FileSlotName(number, 9),
                        FileTime(number, empty=_("Empty Slot")),
                        FileSaveName(number))
    

    text file_text xpos 2 ypos 22 size 10 color "#452d10" kerning 2 font "SHPinscher-Regular.otf"

    key "save_delete" action FileDelete(number)
    



##############################################################################
# Preferences
#
# Screen that allows the user to change the preferences.
# http://www.renpy.org/doc/html/screen_special.html#prefereces

screen preferences:
    
    tag menu

    if main_menu:
        window:
            style "mm_root"

    imagemap:

            ground "gui/settings-idle.png"
            hover "gui/settings-hover.png"
            selected_idle "gui/settings-idleselected.png"
            alpha False
            cache False
            
            hotspot (7,7,28,28) action Return() 

            
            hotspot (71,124,61,15) action Preference("display", "window") 
            hotspot (71,145,74,15) action Preference("display", "fullscreen") 
            
            hotspot (71,199,83,15) action Preference("skip", "toggle")
            hotspot (71,220,93,15) action Preference("after choices", "toggle")
            hotspot (71,241,83,15) action Preference("transitions", "toggle")
            
            
            bar pos (189,134) value Preference("text speed") style "pref_slider"
            bar pos (313,134) value Preference("auto-forward time") style "pref_slider"
            bar pos (189,211) value Preference("music volume") style "pref_slider"
            bar pos (313,211) value Preference("sound volume") style "pref_slider"
            
            
            
            hotspot (97,261,96,60) action ShowMenu("credits")
            hotspot (387,261,96,60) action ShowMenu("load")
    
            
            if not main_menu:
                hotspot (195,261,96,60) action ShowMenu("character_customization_screen")
                hotspot (289,261,96,60) action ShowMenu("save")


init -2 python:
    style.pref_frame.xfill = True
    style.pref_frame.xmargin = 8
    style.pref_frame.top_margin = 5

    style.pref_vbox.xfill = True

    style.pref_button.size_group = "pref"
    style.pref_button.xalign = 1.0

    style.pref_slider.left_bar = "gui/bar-full.png"
    style.pref_slider.right_bar = "gui/bar-empty.png"
    style.pref_slider.hover_left_bar = "gui/bar-full.png"
    style.pref_slider.ymaximum = 8
    style.pref_slider.xmaximum = 91
    style.pref_slider.thumb = "gui/slider-thumb.png"


##############################################################################


# Yes/No Prompt
#
# Screen that asks the user a yes or no question.
# http://www.renpy.org/doc/html/screen_special.html#yesno-prompt

screen yesno_prompt:
    on "show" action With(dissolve)
    zorder 9500
    
    #modal True
    
    add "gui/yesno-background.png"
    
    imagemap:
        ground 'gui/yesno-ground.png'
        idle 'gui/yesno-idle.png' 
        hover 'gui/yesno-hover.png'
        alpha False
        # This is so that everything transparent is invisible to the cursor.
        
        
        hotspot (175, 173, 47, 20) action yes_action 
        hotspot (245, 173, 40, 20) action no_action 
  

    if message == layout.ARE_YOU_SURE:
        add "gui/yesno-areyousure.png"
        
    elif message == layout.LOADING:
        add "gui/yesno-load.png"
        
    elif message == layout.QUIT:
        add "gui/yesno-leavegame.png"
        
    elif message == layout.OVERWRITE_SAVE:
        add "gui/yesno-overwrite.png"
        
    elif message == layout.MAIN_MENU:
        add "gui/yesno-return.png"
        
    elif message == layout.DELETE_SAVE:
        add "gui/yesno-delete.png"
        

init python:
    config.quit_action = Quit()


##############################################################################
# Quick Menu
#
# A screen that's included by the default say screen, and adds quick access to
# several useful functions.
screen quick_menu:

    imagemap:
        ground "gui/textbox-menu-icon-idle.png"
        idle "gui/textbox-menu-icon-idle.png"
        hover "gui/textbox-menu-icon-rollover.png"
        alpha False
        # This is so that everything transparent is invisible to the cursor.

        hotspot (437,285,33,33) action ShowMenu('preferences')
        
        
        
        
## Credits screen ##############################################################
##
## Credits
#
# Screen that gives credit to all the awesome people who helped make this game

screen credits():

    tag menu
    
    if main_menu:
        add "images/backgrounds/Background-by-ansimuz.png"
    
    imagemap:
        ground 'gui/credits-idle.png'
        idle 'gui/credits-idle.png' 
        hover 'gui/credits-hover.png'
        
        hotspot (7,7,28,28) action Return() 
        hotspot (0,261,96,60) action ShowMenu("preferences")
        hotspot (387,261,96,60) action ShowMenu("load")

    
            
        if not main_menu:
            hotspot (195,261,96,60) action ShowMenu("character_customization_screen")
            hotspot (289,261,96,60) action ShowMenu("save")
