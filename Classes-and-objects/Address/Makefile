ifeq ($(OS),Windows_NT)
NAME := Address.exe
else
NAME := Address
endif

CXX := c++
CXXFLAGS := -Wall -Wextra -Werror -std=c++23 -I.

SRCS := Address.cpp main.cpp
OBJS := $(SRCS:.cpp=.o)

.PHONY: all clean fclean re run

all: $(NAME)

$(NAME): $(OBJS)
	$(CXX) $(CXXFLAGS) $(OBJS) -o $(NAME)

%.o: %.cpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

run: $(NAME)
	./$(NAME)

clean:
	$(RM) $(OBJS)

fclean: clean
	$(RM) $(NAME)

re: fclean all
