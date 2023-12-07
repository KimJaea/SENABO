package com.senabo.domain.brushingTeeth.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QBrushingTeeth is a Querydsl query type for BrushingTeeth
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QBrushingTeeth extends EntityPathBase<BrushingTeeth> {

    private static final long serialVersionUID = 584347678L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QBrushingTeeth brushingTeeth = new QBrushingTeeth("brushingTeeth");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QBrushingTeeth(String variable) {
        this(BrushingTeeth.class, forVariable(variable), INITS);
    }

    public QBrushingTeeth(Path<? extends BrushingTeeth> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QBrushingTeeth(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QBrushingTeeth(PathMetadata metadata, PathInits inits) {
        this(BrushingTeeth.class, metadata, inits);
    }

    public QBrushingTeeth(Class<? extends BrushingTeeth> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

