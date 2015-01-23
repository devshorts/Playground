package com.devshorts.enumerable.iterators;

import java.util.function.BiFunction;
import java.util.function.Predicate;
import java.util.function.Supplier;

public class PredicateIterator<TSource> extends MapIterator<TSource, Boolean> {
    private final Predicate<TSource> elementPredicate;
    private final BiFunction<Boolean, Boolean, Boolean> aggregator;
    private Predicate<Boolean> shortCircuitPredicate;
    private Supplier<Boolean> shortCircuitResult;
    private Boolean seed;
    private Boolean done = false;

    public PredicateIterator(Iterable<TSource> input,
                             Predicate<TSource> elementPredicate,
                             BiFunction<Boolean, Boolean, Boolean> aggregator,
                             Predicate<Boolean> shortCircuitPredicate,
                             Supplier<Boolean> shortCircuitResult,
                             Boolean seed) {
        super(input);
        this.elementPredicate = elementPredicate;
        this.aggregator = aggregator;
        this.shortCircuitPredicate = shortCircuitPredicate;
        this.shortCircuitResult = shortCircuitResult;
        this.seed = seed;
    }

    @Override
    public boolean hasNext(){
        return !done;
    }

    public Boolean next(){
        done = true;

        while(source.hasNext()){
            seed = aggregator.apply(seed, elementPredicate.test((TSource) source.next()));

            if(shortCircuitPredicate.test(seed)){
                return shortCircuitResult.get();
            }
        }

        return seed;
    }
}

