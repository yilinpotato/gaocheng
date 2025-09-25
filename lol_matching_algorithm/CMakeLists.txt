cmake_minimum_required(VERSION 3.10)
project(LOLMatchingAlgorithm)

# 设置C++标准
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 设置编译选项
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wextra -O2")

# 添加调试信息
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -g -DDEBUG")

# 包含头文件目录
include_directories(include)

# 收集源文件
file(GLOB SOURCES "src/*.cpp")

# 创建可执行文件
add_executable(lol_matching ${SOURCES})

# 如果需要链接pthread库（用于多线程）
find_package(Threads REQUIRED)
target_link_libraries(lol_matching ${CMAKE_THREAD_LIBS_INIT})

# 设置输出目录
set_target_properties(lol_matching PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
)

# 打印编译信息
message(STATUS "Project: ${PROJECT_NAME}")
message(STATUS "C++ Standard: ${CMAKE_CXX_STANDARD}")
message(STATUS "Compiler: ${CMAKE_CXX_COMPILER}")
message(STATUS "Build Type: ${CMAKE_BUILD_TYPE}")