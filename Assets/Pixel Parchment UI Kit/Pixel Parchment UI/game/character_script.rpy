init python:

    def draw_character(st, at): 
        return LiveComposite(
            (670, 670), 
            (0, 0), "character/"+ characterclass +"/sprite/" +"%s.png"%body ,

            
            ),.1


init:
    image character = DynamicDisplayable(draw_character)
    