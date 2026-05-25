# You can place the script of your game in this file.

init python:
    config.layers = [ 'master', 'transient', 'screens', 'overlay']
    
    credits = ('CODING & UI DESIGN', 'Sasquatchii'), ('BACKGROUND', 'Ansimuz'), ('CHARACTER SPRITES', 'Open Pixel Project'), ('RENPY ENGINE', 'PyTom'), ("{size=30}"'PROGRAMMING COUNSEL', "{size=30}" 'Philat,' "{space=19}" 'Imperf3kt,' "{space=19}" 'TellerFarsight')
    credits_s = "{size=80}Credits\n\n"
    c1 = ''
    for c in credits:
        if not c1==c[0]:
            credits_s += "\n{size=30}" + c[0] + "\n"
        credits_s += "{size=40}" + c[1] + "\n"
        c1=c[0]
    credits_s += "" #Don't forget to set this to your Ren'py version

# Declare images below this line, using the image statement.
# eg. image eileen happy = "eileen_happy.png"

image white = "#ffffff"
# Declare characters used by this game.
define mc = Character('', show_two_window='False')
define sas = Character('Sasquatchii', color="#432e0c", font="Lady Radical.ttf", show_two_window='True', image="sas", who_size=30, who_bold=False)
image sas = "images/charactersprites/none.png"
image side sas = "images/charactersprites/sassprite.png"

#Declare BG images used by this game.
image bg sea = "images/backgrounds/Background-by-ansimuz.png"

init:
    $ flashbulb = Fade(0.2, 0.0, 0.8, color='#fff')

    #    image cred = Text(credits_s, font="myfont.ttf", text_align=0.5) #use this if you want to use special fonts
    image cred = Text(credits_s, text_align=0.5)
    image theend = Text("{size=70}The end", text_align=0.5)
    
    $ Positioncenter = Position(xpos=0.6, ypos=0.8)
    $ Positiondinner = Position(xpos=0.71, ypos=0.74)
    $ Positiondinner1 = Position(xpos=0.68, ypos=0.71)
    $ Positiondinner2 = Position(xpos=0.64, ypos=0.7)
    $ Positionground = Position(xpos=0.59, ypos=0.74)
    $ Positioncrush = Position(xpos=0.5, ypos=0.57)
    
    $ blackout = Fade(0.6, 0.0, 0.9, color='#2A3635')
    
    $ flashbulb2 = Fade(0.1, 0.0, 0.1, color='#c6e6dd') 
    
    $ flashbulb3 = Fade(0.1, 0.0, 0.1, color='#3A4446') 
    
    $ esubtitle = Character(None,
                            what_layout="subtitle",
                            what_xalign=0.5,                           
                            what_text_align=0.5,
                            window_background=None,
                            window_yminimum=77,
                            window_xfill=False,
                            window_xalign=0.5

                            )
    
init python:
    define.move_transitions("move", 1.5)


# The game starts here.
label start:
    scene bg sea

    sas "Hi, I'm Sasquatchii. Thanks for downloading my Pixel Parchment UI Freebie!"

    sas "You can gain access to the entire menu by clicking on the icon to the right. >>>>>>>"
    
    sas "Here's a sneak peak of what the choice options look like!"
    
    menu:
        "That's awesome, they look beautiful.":
             jump lover

        "They look stupid, I hate them!":
             jump hater
             
label lover:
        sas "Thanks friend, I'm so happy to hear that you like them!"
        jump tutorial  
    
label hater:
        sas "That's OK, you can change them yourself using Ren'Py!"
        jump tutorial  
    
    
label tutorial:
    sas "Players can also change the way their own character looks."
             
    show screen character_customization_screen
    mc "Who do you want to be?"
    hide screen character_customization_screen
    show character onlayer overlay:
        pos (18,255)
    mc "Here is the character you chose. Neat, huh?"
    sas "Now try changing your character by opening the menu, going to the character screen, selecting a new character and clicking ready."
    show character onlayer overlay:
        pos (18,255)
    mc "And here's your character again! Players can change their character at any time."
    sas "You can find more info on the other assets included in this UI kit by viewing the PDF included in your download."
    sas "If you'd like to get in touch, you can find me on twitter at {a=https://twitter.com/Sasquatchiix}@Sasquatchiix{/a}."
    sas "If you're new to using Ren'Py and would like to learn more, you can find more information at {a=https://www.renpy.org/}www.renpy.org{/a}."
    sas "The {a=https://lemmasoft.renai.us/forums/index.php?sid=ba315754ac225e73718d7064e6c50346}Lemma Soft Forums{/a} are a good resource if you need help or have questions."
    sas "Thanks again for downloading! Now get out there and make something awesome!"

    label credits:
    $ credits_speed = 16 #scrolling speed in seconds
    scene bg sea #replace this with a fancy background
    with dissolve
    show theend:
        yanchor 0.5 ypos 0.5
        xanchor 0.5 xpos 0.5
    with dissolve
    with Pause(1)
    hide theend
    show cred at Move((0.5, 3.0), (0.5, 0.0), credits_speed, repeat=False, bounce=False, xanchor="center", yanchor="bottom")
    with Pause(credits_speed)

    return
