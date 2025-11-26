project(
    'MoldovanEduard-Informatica-3-GraficaPeCalculator',
    'cpp',
    version : '0.1.0',
    default_options : ['warning_level=3', 'cpp_std=c++20'])

src = []
subdir('src')

# all of this is required to make Meson do the extremely complicated task of copying an entire directory to the build
# dir, as it seems it simply does not understand such a concept (no, fs.copyfile does not preserve directory paths)... :((
python3 = import('python').find_installation()

dummy_meson_input = meson.current_source_dir() / 'shaders' / 'basic_unlit.vert'
shaders_src = meson.current_source_dir() / 'shaders'
shaders_dst = meson.current_build_dir() / 'shaders'

copy_shaders = custom_target(
    'copy_shaders',
    input : dummy_meson_input,
    output : 'shaders.stamp',
    command : [
        python3, '-c',
        '''
import shutil, sys
src, dst = sys.argv[1], sys.argv[2]
shutil.copytree(src, dst, dirs_exist_ok=True)
open(sys.argv[3], "w").write("")
''',
        shaders_src, shaders_dst, '@OUTPUT@'
    ]
)

glfw_dep = dependency('glfw', fallback : ['glfw', 'glfw_dep'])
glm_dep = dependency('glm', fallback : ['glm', 'glm_dep'])
glad_dep = dependency('glad', fallback : ['glad', 'glad_dep'])
shader_files_dep = declare_dependency(sources : copy_shaders)

MoldovanEduard_Informatica_3_GraficaPeCalculator = executable(
    'MoldovanEduard_Informatica_3_GraficaPeCalculator',
    src,
    dependencies : [glfw_dep, glm_dep, glad_dep, shader_files_dep],
    include_directories : include_directories('include'))