project(
    'MoldovanEduard-Informatica-3-GraficaPeCalculator',
    ['cpp', 'c'],
    version : '0.1.0',
    default_options : ['warning_level=3', 'cpp_std=c++20'])

src = []
subdir('src')

glfw_dep = dependency('glfw', fallback : ['glfw', 'glfw_dep'])
glm_dep = dependency('glm', fallback : ['glm', 'glm_dep'])

MoldovanEduard_Informatica_3_GraficaPeCalculator = executable(
    'MoldovanEduard_Informatica_3_GraficaPeCalculator',
    src,
    dependencies : [glfw_dep, glm_dep],
    include_directories : include_directories('include'))