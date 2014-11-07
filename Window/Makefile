IDIR = .
CC=gcc
CFLAGS=-I$(IDIR)

ODIR = obj
LDIR = lib

LIBS=

_DEPS = XWindowEnums.h,XWindowTypes.h,XWindow.h
DEPS = $(patsubst %,$(IDIR)/%,$(_DEPS))

_OBJ = XWindow.o
OBJ = $(patsubst %,$(ODIR)/%,$(_OBJ))


$(ODIR)/%.o: %.c $(DEPS)
	$(CC) -c -o $@ $< $(CFLAGS)

hellomake: $(OBJ)
	$(CC) -o $@ $^ $(CFLAGS) $(LIBS)

.PHONY: clean

clean:
	rm -f $(ODIR)/*.o *~ core $(INCDIR)/*~
