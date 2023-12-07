package com.senabo.domain.expense.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QExpense is a Querydsl query type for Expense
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QExpense extends EntityPathBase<Expense> {

    private static final long serialVersionUID = 1475974110L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QExpense expense = new QExpense("expense");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    public final NumberPath<Double> amount = createNumber("amount", Double.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final StringPath detail = createString("detail");

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final StringPath item = createString("item");

    public final com.senabo.domain.member.entity.QMember memberId;

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QExpense(String variable) {
        this(Expense.class, forVariable(variable), INITS);
    }

    public QExpense(Path<? extends Expense> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QExpense(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QExpense(PathMetadata metadata, PathInits inits) {
        this(Expense.class, metadata, inits);
    }

    public QExpense(Class<? extends Expense> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

