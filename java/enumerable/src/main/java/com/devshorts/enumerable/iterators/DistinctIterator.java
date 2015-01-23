package com.devshorts.enumerable.iterators;

import java.util.HashSet;
import java.util.function.Function;

public class DistinctIterator<TSource, TProjection> extends EnumerableIterator<TSource> {

    private HashSet<TProjection> set = new HashSet<>();

    private TSource last;
    private Function<TSource, TProjection> project;

    public DistinctIterator(Iterable<TSource> input, Function<TSource, TProjection> project) {
        super(input);
        this.project = project;
    }

    @Override
    public boolean hasNext(){
        while(source.hasNext()){
            last = source.next();

            TProjection lastProjection = project.apply(last);

            if(!set.contains(lastProjection)){
                set.add(lastProjection);
                return true;
            }
        }

        return false;
    }

    @Override
    public TSource next(){
        return last;
    }
}
