package com.devshorts.enumerable.iterators;

import java.util.function.Predicate;

public class FilterIterator<TSource> extends EnumerableIterator<TSource> {

    private int idx = 0;
    private TSource nextItem = null;
    private Predicate<TSource> filterFunc;

    public FilterIterator(Iterable<TSource> input, Predicate<TSource> filterFunc) {
        super(input);
        this.filterFunc = filterFunc;
    }

    @Override
    public boolean hasNext() {
        while(source.hasNext()){
            nextItem = (TSource)source.next();
            idx++;
            if(filterFunc.test(nextItem)){
                return true;
            }
        }

        return false;
    }

    @Override
    public TSource next() {
        return nextItem;
    }
}
