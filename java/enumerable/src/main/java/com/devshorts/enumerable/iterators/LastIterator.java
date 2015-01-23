package com.devshorts.enumerable.iterators;

public class LastIterator<TSource> extends EnumerableIterator<TSource> {

    TSource last;
    Boolean emitted = false;

    public LastIterator(Iterable<TSource> input) {
        super(input);
    }

    @Override
    public boolean hasNext(){
        if(emitted){
            return false;
        }

        while(source.hasNext()){
            last = source.next();
        }

        return last != null;
    }

    @Override
    public TSource next(){
        emitted = true;

        return last;
    }
}
