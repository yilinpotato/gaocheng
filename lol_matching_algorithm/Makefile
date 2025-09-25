# LOL匹配算法 Makefile
CXX = g++
CXXFLAGS = -std=c++17 -Wall -Wextra -O2 -Iinclude
DEBUGFLAGS = -g -DDEBUG
LDFLAGS = -pthread

# 目录定义
SRCDIR = src
INCDIR = include
BUILDDIR = build
BINDIR = bin

# 源文件和目标文件
SOURCES = $(wildcard $(SRCDIR)/*.cpp)
OBJECTS = $(SOURCES:$(SRCDIR)/%.cpp=$(BUILDDIR)/%.o)
TARGET = $(BINDIR)/lol_matching

# 默认目标
all: $(TARGET)

# 创建可执行文件
$(TARGET): $(OBJECTS) | $(BINDIR)
	$(CXX) $(OBJECTS) -o $@ $(LDFLAGS)
	@echo "构建完成: $@"

# 编译源文件
$(BUILDDIR)/%.o: $(SRCDIR)/%.cpp | $(BUILDDIR)
	$(CXX) $(CXXFLAGS) -c $< -o $@

# 创建目录
$(BUILDDIR):
	mkdir -p $@

$(BINDIR):
	mkdir -p $@

# 调试版本
debug: CXXFLAGS += $(DEBUGFLAGS)
debug: $(TARGET)

# 清理
clean:
	rm -rf $(BUILDDIR) $(BINDIR)
	@echo "清理完成"

# 运行程序
run: $(TARGET)
	./$(TARGET)

# 安装（复制到系统目录）
install: $(TARGET)
	cp $(TARGET) /usr/local/bin/
	@echo "安装完成"

# 帮助信息
help:
	@echo "可用的make目标:"
	@echo "  all      - 构建程序（默认）"
	@echo "  debug    - 构建调试版本"
	@echo "  clean    - 清理构建文件"
	@echo "  run      - 构建并运行程序"
	@echo "  install  - 安装程序到系统"
	@echo "  help     - 显示此帮助信息"

# 声明伪目标
.PHONY: all debug clean run install help

# 显示编译信息
info:
	@echo "编译器: $(CXX)"
	@echo "编译选项: $(CXXFLAGS)"
	@echo "源文件: $(SOURCES)"
	@echo "目标文件: $(OBJECTS)"
	@echo "可执行文件: $(TARGET)"