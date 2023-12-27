#-------------------------------------------------------------------------------------------------------------------------
# ________  ___       _______   ________   ________  _______   ________          ___       ___  ________   ___  __       
#|\   __  \|\  \     |\  ___ \ |\   ___  \|\   ___ \|\  ___ \ |\   __  \        |\  \     |\  \|\   ___  \|\  \|\  \     
#\ \  \|\ /\ \  \    \ \   __/|\ \  \\ \  \ \  \_|\ \ \   __/|\ \  \|\  \       \ \  \    \ \  \ \  \\ \  \ \  \/  /|_   
# \ \   __  \ \  \    \ \  \_|/_\ \  \\ \  \ \  \ \\ \ \  \_|/_\ \   _  _\       \ \  \    \ \  \ \  \\ \  \ \   ___  \  
#  \ \  \|\  \ \  \____\ \  \_|\ \ \  \\ \  \ \  \_\\ \ \  \_|\ \ \  \\  \|       \ \  \____\ \  \ \  \\ \  \ \  \\ \  \ 
#   \ \_______\ \_______\ \_______\ \__\\ \__\ \_______\ \_______\ \__\\ _\        \ \_______\ \__\ \__\\ \__\ \__\\ \__\
#    \|_______|\|_______|\|_______|\|__| \|__|\|_______|\|_______|\|__|\|__|        \|_______|\|__|\|__| \|__|\|__| \|__|
#                                                                                                                        
#-------------------------------------------------------------------------------------------------------------------------
#                                                    writen by Nori_CS
#                                                https://github.com/NoriteSC

#import section
import os
import sys
import bpy

#code section
argv = sys.argv
def GetArg(argname): 
    if argname in argv: 
        return argv.index(argname) + 1
    else: 
        return -1

def main():
    print("[Blender Link] ExtractAnimationData.py has started execution")
    print("[Blender Link] [Flax -> Blender] One way hand sake started")

    # grab the ProjectFolder path from args
    pfid = GetArg("--ProjectFolder")
    if pfid != -1:
        ProjectFolder = argv[pfid]
        ProjectCacheFolder =  os.path.join(ProjectFolder,"Cache")
        print("[Blender Link]\nProjectCacheFolder: "+ ProjectCacheFolder)
        itemid = GetArg("--ContentItem")
        if itemid != -1:
            item = argv[itemid][8:]
            print(item)
            path = os.path.join(ProjectCacheFolder,"Blender Link Cache",item)+".BLTCashe"
            directory = os.path.dirname(path)
            # Check whether the specified path exists or not
            if not os.path.exists(directory):
               # Create a new directory because it does not exist
               os.makedirs(directory)
               
            f = open(path, "wt")
            for obj in bpy.data.objects:
                if obj.animation_data:
                    if len(obj.animation_data.nla_tracks) != 0:
                        f.write("-X " + obj.name + "\n")
                        for obj in obj.animation_data.nla_tracks:
                            for action in obj.strips:
                                f.write("--X " + action.name + "\n")

            f.close()
        else:
            sys.stderr.write("[Blender Link] Faled to get the ContentItem path")

    print("[Blender Link] [Blender -> Flax] One way hand sake started")
    
main()