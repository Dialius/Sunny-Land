define character_img = "character"

screen character_customization_screen:
    modal True

    # The main menu buttons.
    
    imagemap:
        ground "gui/characterselect-idle.png"
        idle "gui/characterselect-idle.png"
        hover "gui/characterselect-hover.png"
        selected_idle "gui/characterselect-selectedidle.png"  
        alpha False # This is so that everything transparent is invisible to the cursor.
        
        hotspot (7,7,28,28) action Return()        
        
        hotspot (63,98,55,62) action [ SetVariable( "characterclass", "Wildman")]
        hotspot (125,98,55,62) action [ SetVariable( "characterclass", "skeleton")]
        hotspot (187,98,55,62) action [ SetVariable( "characterclass", "dog")]
        hotspot (249,98,55,62) action [ SetVariable( "characterclass", "druid")]
        hotspot (311,98,55,62) action [ SetVariable( "characterclass", "wisewoman")]
        hotspot (373,98,55,62) action [ SetVariable( "characterclass", "pilot")]
                
        hotspot (63,172,55,62) action [ SetVariable( "characterclass", "islandman")]
        hotspot (125,172,55,62) action [ SetVariable( "characterclass", "islandwoman")]
        hotspot (187,172,55,62) action [ SetVariable( "characterclass", "archeologist")]  
        hotspot (249,172,55,62) action [ SetVariable( "characterclass", "girl")]
        hotspot (311,172,55,62) action [ SetVariable( "characterclass", "mechanic")]
        hotspot (373,172,55,62) action [ SetVariable( "characterclass", "nerd")]

              
        hotspot (382,234,98,36) action Return()
        
        
        hotspot (0,261,96,60) action ShowMenu("preferences")
        hotspot (97,261,96,60) action ShowMenu("credits")
        hotspot (289,261,96,60) action ShowMenu("save")
        hotspot (387,270,94,51) action ShowMenu("load")

           