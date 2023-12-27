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

def ExportAction(obj,NlaStrip,ExportTo):
    print("Exportting Action:" +NlaStrip.name)
    bpy.ops.object.select_all(action='DESELECT')
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    obj.animation_data.action = NlaStrip.action
    bpy.ops.export_scene.fbx(
    filepath= os.path.join(ExportTo , NlaStrip.action.name) + ".fbx",
    use_selection=True,
    add_leaf_bones=False,
    bake_anim_use_nla_strips=True,
    bake_anim_use_all_actions=False,
    object_types = {'ARMATURE'})

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
            path = os.path.join(ProjectCacheFolder,"Blender Link Cache",item)+".BLTCashe"
            itempath = os.path.join(ProjectFolder,os.path.dirname(argv[itemid]),"Animations")
            # Check whether the specified path exists or not
            if not os.path.exists(itempath):
               # Create a new directory because it does not exist
               os.makedirs(itempath)
            #return
            f = open(path, "r")

            for obj in bpy.data.objects:
                if obj.animation_data:
                    if len(obj.animation_data.nla_tracks) != 0:
                        for nla_tracks in obj.animation_data.nla_tracks:
                            for NlaStrip in nla_tracks.strips:
                                NlaStrip.mute = True
                        #f.write("-X " + obj.name + "\n")
                        if f.readline()[1] == 'O':
                            # Export all
                            for nla_tracks in obj.animation_data.nla_tracks:
                                for NlaStrip in nla_tracks.strips:
                                    NlaStrip.mute = False
                                    ExportAction(obj,NlaStrip,itempath)
                                    NlaStrip.mute = True
                        else:
                            # Export selected
                            for nla_tracks in obj.animation_data.nla_tracks:
                                for NlaStrip in nla_tracks.strips:
                                    line = f.readline()
                                    if line[2] == 'O':
                                        if line[4:] == NlaStrip.name:
                                            NlaStrip.mute = False
                                            ExportAction(obj,NlaStrip,itempath)
                                            NlaStrip.mute = True

            f.close()
        else:
            sys.stderr.write("[Blender Link] Faled to get the ContentItem path")

    print("[Blender Link] [Blender -> Flax] One way hand sake started")
    
main()