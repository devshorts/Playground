package com.devshorts.enumerable.iterators;

public class NthIterator<TSource> extends EnumerableIterator<TSource> {
    private boolean firstReturned = false;
    TSource nth;
    private Boolean emitted = false;
    private int n;

    public NthIterator(Iterable<TSource> input, int n) {
        super(input);
        this.n = n;
    }

    @Override
    public boolean hasNext(){
        if(emitted){
            return false;
        }

        while(n >= 1 && source.hasNext()){
            nth = source.next();

            n--;
        }

        return nth != null;
    }

    @Override
    public TSource next(){
        emitted = true;
        return nth;
    }
}
